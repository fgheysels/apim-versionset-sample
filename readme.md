# Introduction

This repository contains samples on how to use versioned ASP.NET API's with Azure API Management VersionSets.

There are 2 ASP.NET API's present in this repository:

- src/single-api-multiple-versions

  This folder contains an ASP.NET sample API for which one Open API Specification file can be generated per version that is defined in the API. 

- src/multiple-apis-multiple-versions

  This folder contains a slightly more complicated ASP.NET sample.  This single ASP.NET API project contains multiple API definitions and each of them is versioned.  In other words, instead of just generating one Open API spec file per version, an Open API spec file is generated for each 'API group' and for each version.

For detailed information about the source code, please see the readme.md file that is present in each folder.

# Getting started

This section outlines the steps to deploy the samples to Azure.
In a real life scenario, all these steps should be automated via deployment pipelines but that's not the goal of this sample.

## Create an Azure Web App

Create an Azure Web App.  There are multiple ways to do that; you can do it straightforward via the Azure Portal, by deploying a bicep file that describes your app or via the following Azure CLI commands:

```
az group create --name apiversionset-test --location "West Europe"
az appservice plan create --name fg-appsvc-test --resource-group apiversionset-test --location "West Europe" --sku B1
az webapp create --name fg-apiversionset-test --resource-group apiversionset-test --plan fg-appsvc-test --runtime dotnet:6
```

> It's not mandatory to deploy to a Web App to get this sample up and running. The API can be deployed in a Web App for Containers or Container App as well.  For simplicity, an Azure Web App is used.

## Deploy binaries to Azure

The simplest way to deploy the API binaries to the Azure Web App that has just been created, is by doing this from Visual Studio.NET.  Again, in a real project, deployment should be done via pipelines.
When deploying via the 'Publish' wizard in VS.NET, skip the 'API management' step.  We'll define the API in API Management via a separate command.

When publishing the API succeeded, you should be able to see the Swagger UI on the deployed web-app by navigating to `https://<yourwebappname>.azurewebsites.net/api/docs`

## Create an Azure API Management service

Since this sample is about deploying versioned API's in Azure API Management, an API Management instance is required.  Again, there are multiple ways to create an API Management instance.  Be aware that creating an APIM instance takes quite some time.

## Create a VersionSet in APIM

To deploy a VersionSet to APIM, you can deploy a bicep file that describes the versionset.  
See the `apim_versionset.bicep` file that can be found in the `deploy` folder.

Deploy the bicep template using this command:

```
 az deployment group create --name versionset_deploy --resource-group <resourcegroup> --template-file .\apim_versionset.bicep --parameters apim_name=<apim_name> 
```

This bicep file will define 3 version-sets in the API Management instance:

- cars_api-versionset
- vessels_api-versionset
- transport_api-versionset

The `cars_api-versionset` and `vessels_api-versionset` are used for the `multiple-apis-multiple-versions` sample.
The `transport_api-versionset` is used for the `singe-api-multiple-versions` sample.

## Build Open API spec files

We need to generate the open-api spec for every version of our API.  

In the `multiple-apis-multiple-versions` sample, the sample Web App has actually 2 different API's defined and there are 2 versions for every API.  This means there will be in fact 4 open API spec files.
In the `single-api-multiple-versions` sample, the sample Web App has just 2 different versions of the API.  This means 2 Open API spec files.

Before we can generate the Open API spec file on the command-line, we first need to install a tool:

```
dotnet new tool-manifest --force
dotnet tool install swashbuckle.aspnetcore.cli --version 6.5.0
```
> The version that is specified here, must match with the version of the `Swashbuckle.AspnetCore` package that is referenced in the ASP.NET project.

Afterwards, the Open API spec files can be generated.
For the `multiple-apis-multiple-versions` sample, execute these commands:

```
dotnet swagger tofile --output c:/temp/api_cars_v1-openapi.json Fg.Samples.MultipleApiVersions/bin/Debug/net6.0/Fg.Samples.MultipleApiVersions.dll cars_v1

dotnet swagger tofile --output c:/temp/api_cars_v2-openapi.json Fg.Samples.MultipleApiVersions/bin/Debug/net6.0/Fg.Samples.MultipleApiVersions.dll cars_v2

dotnet swagger tofile --output c:/temp/api_vessels_v1-openapi.json Fg.Samples.MultipleApiVersions/bin/Debug/net6.0/Fg.Samples.MultipleApiVersions.dll vessels_v1

dotnet swagger tofile --output c:/temp/api_vessels_v2-openapi.json Fg.Samples.MultipleApiVersions/bin/Debug/net6.0/Fg.Samples.MultipleApiVersions.dll vessels_v2
```

> Note that the last part of the above command each time represents a 'GroupName' that has been defined in the `ApiExplorerSettings` attribute.

> If the `dotnet swagger` command fails, make sure that you specify the .NET SDK that must be used by creating a `global.json` file that refers to the correct .NET SDK that is installed on your system.
> List the installed SDK's via `dotnet --list-sdks`
> Create the global.json file via `dotnet new globaljson --sdk-version <version> --roll-forward latestMinor`.

For the `single-api-multiple-versions` sample, execute these commands:

```
dotnet swagger tofile --output c:/temp/api_transport_v1-openapi.json Fg.Samples.SingleApiMultipleVersions/bin/Debug/net6.0/Fg.Samples.SingleApiMultipleVersions.dll v1

dotnet swagger tofile --output c:/temp/api_transport_v2-openapi.json Fg.Samples.SingleApiMultipleVersions/bin/Debug/net6.0/Fg.Samples.SingleApiMultipleVersions.dll v2
```

> Note that the last part of the above command just specifies the version.  That's because the version-name defines the 'groupname'.

> If the `dotnet swagger` command fails, make sure that you specify the .NET SDK that must be used by creating a `global.json` file that refers to the correct .NET SDK that is installed on your system.
> List the installed SDK's via `dotnet --list-sdks`
> Create the global.json file via `dotnet new globaljson --sdk-version <version> --roll-forward latestMinor`.

## Import Open API specs in APIM

Once the Open API specs are created, they can be imported in APIM using the `Import-ApimVersioned-Api.ps1` Powershell script that is found in the `scripts` folder.

This script will make some changes the the paths that are defined in the Open API specification.  This is necessary because the path that is exposed by APIM already contains the API version information, since the API path must be unique in APIM.
However, if that version information is also present in the paths mentionned in the Open API spec, this results in incorrect API paths. 

### multiple-apis-multiple-versions

To import the Open API spec files for the `multiple-apis-multiple-versions` sample, execute these commands from the `scripts` folder:

For cars-api v1:
```
.\Import-ApiVersioned-Api.ps1 `
  -Apim_ResourceGroup apiversionset-test `
  -Apim_Name <your-apim-name> `
  -Api_Id cars_api ` # note that this corresponds with a part of the versionset-name in the bicep file
  -Api_Path cars `
  -Api_DisplayName "cars api" `
  -Api_Version v1 `
  -OpenApi_File c:/temp/api_cars_v1-openapi.json `
  -Api_SwaggerPrefix /api/v1 ` # This is the part that must be removed from the endpoint-paths in the OpenAPI spec
  -Service_BackendUrl https://<yourwebappname>.azurewebsites.net
```

For cars-api v2:
```
.\Import-ApiVersioned-Api.ps1 `
  -Apim_ResourceGroup apiversionset-test `
  -Apim_Name <your-apim-name> `
  -Api_Id cars_api ` # note that this corresponds with a part of the versionset-name in the bicep file
  -Api_Path cars `
  -Api_DisplayName "cars api" `
  -Api_Version v2 `
  -OpenApi_File c:/temp/api_cars_v2-openapi.json `
  -Api_SwaggerPrefix /api/v2 ` # This is the part that must be removed from the endpoint-paths in the OpenAPI spec
  -Service_BackendUrl https://<yourwebappname>.azurewebsites.net
```

For vessels-api v1:
```
.\Import-ApiVersioned-Api.ps1 `
  -Apim_ResourceGroup apiversionset-test `
  -Apim_Name <your-apim-name> `
  -Api_Id vessels_api ` # note that this corresponds with a part of the versionset-name in the bicep file
  -Api_Path vessels `
  -Api_DisplayName "vessels api" `
  -Api_Version v1 `
  -OpenApi_File c:/temp/api_vessels_v1-openapi.json `
  -Api_SwaggerPrefix /api/v1 ` # This is the part that must be removed from the endpoint-paths in the OpenAPI spec
  -Service_BackendUrl https://<yourwebappname>.azurewebsites.net
```

For vessels-api v2:
```
.\Import-ApiVersioned-Api.ps1 `
  -Apim_ResourceGroup apiversionset-test `
  -Apim_Name <your-apim-name> `
  -Api_Id vessels_api ` # note that this corresponds with a part of the versionset-name in the bicep file
  -Api_Path vessels `
  -Api_DisplayName "vessels api" `
  -Api_Version v2 `
  -OpenApi_File c:/temp/api_vessels_v2-openapi.json `
  -Api_SwaggerPrefix /api/v2 ` # This is the part that must be removed from the endpoint-paths in the OpenAPI spec
  -Service_BackendUrl https://<yourwebappname>.azurewebsites.net
```

### single-api-multiple-versions

To import the Open API spec files for the `single-api-multiple-versions` sample, execute these commands from the `scripts` folder:

For v1:
```
.\Import-ApiVersioned-Api.ps1 `
  -Apim_ResourceGroup apiversionset-test `
  -Apim_Name <your-apim-name> `
  -Api_Id transport_api ` # note that this corresponds with a part of the versionset-name in the bicep file
  -Api_Path transport `
  -Api_DisplayName "transport api" `
  -Api_Version v1 `
  -OpenApi_File c:/temp/api_transport_v1-openapi.json `
  -Api_SwaggerPrefix /api/v1 ` # This is the part that must be removed from the endpoint-paths in the OpenAPI spec
  -Service_BackendUrl https://<yourwebappname>.azurewebsites.net
```

For v2:
```
.\Import-ApiVersioned-Api.ps1 `
  -Apim_ResourceGroup apiversionset-test `
  -Apim_Name fg-apim `
  -Api_Id transport_api ` # note that this corresponds with a part of the versionset-name in the bicep file
  -Api_Path transport `
  -Api_DisplayName "transport api" `
  -Api_Version v2 `
  -OpenApi_File c:/temp/api_transport_v2-openapi.json `
  -Api_SwaggerPrefix /api/v2` # This is the part that must be removed from the endpoint-paths in the OpenAPI spec
  -Service_BackendUrl https://fg-apiversionset-test.azurewebsites.net
  ```