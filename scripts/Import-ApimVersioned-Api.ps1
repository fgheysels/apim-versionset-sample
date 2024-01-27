param (
    [string] $Apim_ResourceGroup,
    [string] $Apim_Name,
    [string] $Api_Id,
    [string] $Api_Path,
    [string] $Api_DisplayName,
    [string] $Api_Version,
    [string] $OpenApi_File,
    [string] $Api_SwaggerPrefix,
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