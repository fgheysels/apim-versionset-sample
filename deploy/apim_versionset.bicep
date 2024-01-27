param  apim_name string

resource apim 'Microsoft.ApiManagement/service@2023-03-01-preview' existing = {
  name: apim_name
}

resource apim_cars_versionset 'Microsoft.ApiManagement/service/apiVersionSets@2023-03-01-preview' = {
  name: 'cars_api-versionset'
  parent: apim
  properties: {
    description: 'Cars API'
    displayName: 'Cars API'
    versioningScheme: 'Segment'
  }
}

resource apim_vessels_versionset 'Microsoft.ApiManagement/service/apiVersionSets@2023-03-01-preview' = {
  name: 'vessels_api-versionset'
  parent: apim
  properties: {
    description: 'Vessels API'
    displayName: 'Vessels API'
    versioningScheme: 'Segment'
  }
}
