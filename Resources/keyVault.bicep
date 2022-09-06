param name string
param location string = resourceGroup().location

var keyVault = {
  name: 'kv-${name}'
  location: location
}

resource kv 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: keyVault.name
  location: keyVault.location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    accessPolicies: []
    enabledForDeployment: true
    enabledForDiskEncryption: true
    enabledForTemplateDeployment: true
    enableSoftDelete: true
  }
}

output id string = kv.id
output name string = kv.name
