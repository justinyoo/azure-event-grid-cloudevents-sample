param name string
param location string = resourceGroup().location

var eventGrid = {
  name: 'evtgrd-${name}-topic'
  location: location
}

resource evtgrd 'Microsoft.EventGrid/topics@2022-06-15' = {
  name: eventGrid.name
  location: eventGrid.location
  properties: {
    inputSchema: 'CloudEventSchemaV1_0'
  }
}

output id string = evtgrd.id
output name string = evtgrd.name
output endpoint string = reference(evtgrd.id, '2022-06-15').properties.endpoint
output accessKey string = listKeys(evtgrd.id, '2022-06-15').key1
