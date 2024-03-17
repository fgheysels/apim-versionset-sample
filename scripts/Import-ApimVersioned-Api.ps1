param (
    [Parameter(Mandatory=$true)]
    [string] $Apim_ResourceGroup,

    [Parameter(Mandatory=$true)]
    [string] $Apim_Name,

    [Parameter(Mandatory=$true)]
    [string] $Api_Id,

    [Parameter(Mandatory=$true)]
    [string] $Api_Path,

    [Parameter(Mandatory=$true)]
    [string] $Api_DisplayName,

    [Parameter(Mandatory=$true)]
    [string] $Api_Version,

    [Parameter(Mandatory=$true)]
    [string] $OpenApi_File,

    [Parameter(Mandatory=$true)]
    [string] $Api_SwaggerPrefix,
    
    [Parameter(Mandatory=$true)]
    [string] $Service_BackendUrl
)

(Get-Content "$OpenApi_File").Replace($Api_SwaggerPrefix, '') | Set-Content "$OpenApi_File"

az apim api import `
  --resource-group $Apim_ResourceGroup `
  --service-name $Apim_Name `
  --path $Api_Path `
  --api-version-set-id $Api_Id"-VersionSet" `
  --api-id "$Api_Id-$Api_Version" `
  --display-name "$Api_DisplayName" `
  --api-version "$Api_Version" `
  --specification-format OpenApiJson `
  --specification-path "$OpenApi_File" `
  --api-type http `
  --service-url "$Service_BackendUrl$Api_SwaggerPrefix"