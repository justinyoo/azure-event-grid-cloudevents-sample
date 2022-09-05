param name string
param location string = resourceGroup().location

var storage = {
  name: 'st${name}'
  location: location
}

resource st 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  name: storage.name
  location: storage.location
  kind: 'StorageV2'
  sku: {
      name: 'Standard_LRS'
  }
  properties: {
      supportsHttpsTrafficOnly: true
  }
}

output id string = st.id
output name string = st.name
output connectionString string = 'DefaultEndpointsProtocol=https;AccountName=${st.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(st.id, '2021-06-01').keys[0].value}'
