# Multiple Open Api Documents Sample

This sample ASP.NET WebApi project shows how we can have one single backend API which has multiple versions.

## How it works

This sample uses the `Asp.Versioning.Mvc.ApiExplorer` nuget package to apply versioning to our API.


In the `Program.cs` file we specify that versioning must be applied:

```csharp
builder.Services.AddApiVersioning(options =>
    {
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = false;
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddControllers();
```

With the above code we specify that versioning must be applied in the URL path.

For every API version that we support, an Open API spec file must be generated.  We also make sure that the Swagger UI can read those open API specs:

```csharp
builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("cars_v1", new OpenApiInfo() { Title = "Cars API", Version = "v1" });
    config.SwaggerDoc("vessels_v1", new OpenApiInfo() { Title = "Vessels API", Version = "v1" });
    config.SwaggerDoc("cars_v2", new OpenApiInfo() { Title = "Cars API", Version = "v2" });
    config.SwaggerDoc("vessels_v2", new OpenApiInfo() { Title = "Vessels API", Version = "v2" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => { options.RouteTemplate = "api/docs/{documentName}/docs.json"; });
    app.UseSwaggerUI(config =>
    {
        config.SwaggerEndpoint("cars_v1/docs.json", "Cars API v1 docs");
        config.SwaggerEndpoint("vessels_v1/docs.json", "Vessels API v1 docs");
        config.SwaggerEndpoint("cars_v2/docs.json", "Cars API v2 docs");
        config.SwaggerEndpoint("vessels_v2/docs.json", "Cars API v2 docs");
        config.RoutePrefix = "api/docs";
    });
}
```

For every API version that we support, we have a separate folder under the 'Controllers' section.
Each controller has an `ApiVersion` attribute that specifies the API version.

To avoid that we need to 'manually' add every SwaggerDoc and SwaggerUI endpoint as in the code snippet above, this sample contains 3 classes to automate this:

- `VersionedApiDescriptionProvider`
- `ConfigureSwaggerOptions`
- `ConfigureSwaggerUiOptions`

The `VersionedApiDescriptionProvider` combines the ApiExplorerGroups with the Version information so that an Open API specification document is created for each version of every 'Api Explorer Group'.

The `ConfigureSwaggerOptions` and `ConfigureSwaggerUiOptions` contain the code that you would write in the `AddSwaggerGen` and `UseSwaggerUI` methods respectively.
The code can be added via an `Action` in those methods, but you would need to create a `ServiceProvider`.  Building a `ServiceProvider` is a bad practice, as this means that Singleton services that are added in the DI container are being instantiated.
Therefore, the `ConfigureSwaggerOptions` and `ConfigureSwaggerUiOptions` are introduced where the existing `ServiceProvider` is injected.  These 2 classes are added to the DI container itself so they are called when the `AddSwaggerGen` and `UseSwaggerUI` methods are executed.

## Deploy to API Management

### Deploy API to an Azure Web App

For demo purposes, you can simply create a Web App in your Azure Subscription and publish the Web Application from Visual Studio to that Azure Web Application.

When publishing the API succeeded, you should be able to see the Swagger UI on the deployed web-app by navigating to `https://<yourwebappname>.azurewebsites.net/api/docs`

### Create a VersionSet in APIM

To deploy a VersionSet to APIM, you can deploy a bicep file that describes the versionset.  
See the `apim_versionset.bicep` file that can be found in the `bicep` folder.

Deploy the bicep template using this command:

```
 az deployment group create --name example_deploy --resource-group <resourcegroup> --template-file .\apim_versionset.bicep --parameters apim_name=<apim_name>
```

Next, we need to import the Open API spec files and assign them to the versionset.
This is something that is best done using an Azure CLI command: `az apim api import`.  However, when we want to assign the API spec to a versionset, we need to specify the version-set ID.

To retrieve the ID of the version-set, we can use this Azure CLI command:

```
$versionset = az apim api versionset list --resource-group '<resourcegroup>' --service-name '<apim-name>' | ConvertFrom-Json | Where-Object { $_.name -eq "irp_api" }
```

#### Import Open API spec in API Management

First of all, we need to generate the open-api spec file from our API.
We need to generate the open-api spec for every version of our API.  In our sample Web App we have actually 2 different API's defined and we have 2 versions for every API.  This means we have in fact 4 open API specs:

Before we can generate the Open API spec file on the command-line, we first need to install a tool:

```
dotnet new tool-manifest --force
dotnet tool install swashbuckle.aspnetcore.cli --version 6.5.0
```

Afterwards, the Open API spec files can be generated:

```
dotnet swagger tofile --output c:/temp/api_cars_v1-openapi.json Fg.Samples.MultipleApiVersions/bin/Debug/net6.0/Fg.Samples.MultipleApiVersions.dll cars_v1

dotnet swagger tofile --output c:/temp/api_cars_v2-openapi.json Fg.Samples.MultipleApiVersions/bin/Debug/net6.0/Fg.Samples.MultipleApiVersions.dll cars_v2

dotnet swagger tofile --output c:/temp/api_vessels_v1-openapi.json Fg.Samples.MultipleApiVersions/bin/Debug/net6.0/Fg.Samples.MultipleApiVersions.dll vessels_v1

dotnet swagger tofile --output c:/temp/api_vessels_v2-openapi.json Fg.Samples.MultipleApiVersions/bin/Debug/net6.0/Fg.Samples.MultipleApiVersions.dll vessels_v2
```

> Note that the last part of the above command each time represents a 'GroupName' that has been defined in the `ApiExplorerSettings` attribute.

Once the Open API specs are created, they can be imported in APIM using the `Import-ApimVersioned-Api.ps1` Powershell script.

This script will make some changes the the paths that are defined in the Open API specification.  This is necessary because the path that is exposed by APIM already contains the API version information, since the API path must be unique in APIM.
However, if that version information is also present in the paths mentionned in the Open API spec, this results in incorrect API paths.