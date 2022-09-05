param name string
param location string = resourceGroup().location

@secure()
param logicAppEndpointUrl string

var logicApp = {
  endpointUrl: logicAppEndpointUrl
}

var eventGrid = {
  topicName: 'evtgrd-${name}-topic'
  subName: 'evtgrd-${name}-sub'
  location: location
}

resource topic 'Microsoft.EventGrid/topics@2022-06-15' existing = {
  name: eventGrid.topicName
}

resource evtgrd 'Microsoft.EventGrid/eventSubscriptions@2022-06-15' = {
  name: eventGrid.subName
  scope: topic
  properties: {
    eventDeliverySchema: 'CloudEventSchemaV1_0'
    destination: {
      endpointType: 'WebHook'
      properties: {
        endpointUrl: logicApp.endpointUrl
      }
    }
    filter: {
      includedEventTypes: null
    }
  }
}

output id string = evtgrd.id
output name string = evtgrd.name
