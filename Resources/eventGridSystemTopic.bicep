param name string
param location string = resourceGroup().location

param resourceId string
param resourceType string = 'Microsoft.KeyVault.vaults'

var eventGrid = {
  name: 'evtgrd-${name}-topic-system'
  location: location
  topicSource: resourceId
  topicType: resourceType
}

resource evtgrd 'Microsoft.EventGrid/systemTopics@2022-06-15' = {
  name: eventGrid.name
  location: eventGrid.location
  properties: {
    source: eventGrid.topicSource
    topicType: eventGrid.topicType
  }
}

output id string = evtgrd.id
output name string = evtgrd.name
