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

For every API version that we support, an Open API spec file must be generated.  We also make sure that the Swagger UI can read those open API specs.
You could write something like this:

```csharp
builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo() { Title = "Transport API", Version = "v1" });
    config.SwaggerDoc("v2", new OpenApiInfo() { Title = "Transport API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => { options.RouteTemplate = "api/docs/{documentName}/docs.json"; });
    app.UseSwaggerUI(config =>
    {
        config.SwaggerEndpoint("v1/docs.json", "Transport API v1 docs");
        config.SwaggerEndpoint("v2/docs.json", "Transport API v2 docs");
        config.RoutePrefix = "api/docs";
    });
}
```

With this approach, you need to manually 'extend' this to make sure that if a new version is defined, it's being added here as well.

For every API version that we support, we have a separate folder under the 'Controllers' section.
Each controller has an `ApiVersion` attribute that specifies the API version.

To avoid that we need to 'manually' add every SwaggerDoc and SwaggerUI endpoint as in the code snippet above, this sample contains 3 classes to automate this:

- `ConfigureSwaggerOptions`
- `ConfigureSwaggerUiOptions`

The `ConfigureSwaggerOptions` and `ConfigureSwaggerUiOptions` contain the code that you would write in the `AddSwaggerGen` and `UseSwaggerUI` methods respectively.
The code can be added via an `Action` in those methods, but you would need to create a `ServiceProvider`.  Building a `ServiceProvider` is a bad practice, as this means that Singleton services that are added in the DI container are being instantiated.
Therefore, the `ConfigureSwaggerOptions` and `ConfigureSwaggerUiOptions` are introduced where the existing `ServiceProvider` is injected.  These 2 classes are added to the DI container itself so they are called when the `AddSwaggerGen` and `UseSwaggerUI` methods are executed.

## Deploy to API Management

Please see the readme.md file in the root of this project on how to deploy this versioned API to Azure API Management.