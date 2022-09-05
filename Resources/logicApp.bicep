param name string
param location string = resourceGroup().location

var logicApp = {
  name: 'logapp-${name}'
  location: location
}

resource logapp 'Microsoft.Logic/workflows@2019-05-01' = {
  name: logicApp.name
  location: logicApp.location
  properties: {
    state: 'Enabled'
    definition: {
      '$schema': 'https://schema.management.azure.com/providers/Microsoft.Logic/schemas/2016-06-01/workflowdefinition.json#'
      contentVersion: '1.0.0.0'
      parameters: {}
      triggers: {
        manual: {
          type: 'Request'
          kind: 'Http'
          inputs: {
            method: 'POST'
            schema: {}
          }
        }
      }
    }
  }
}

output id string = logapp.id
output name string = logapp.name
output endpoint string = listCallbackUrl('${logapp.id}/triggers/manual', '2019-05-01').value
