param name string
param location string = resourceGroup().location

param eventGridTopicName string
@allowed([
  'system'
  'custom'
])
param eventGridTopicType string = 'custom'
@secure()
param logicAppEndpointUrl string

var logicApp = {
  endpointUrl: logicAppEndpointUrl
}

var eventGrid = {
  topicName: eventGridTopicName
  topicType: eventGridTopicType
  subName: 'evtgrd-${name}-sub'
  location: location
}

resource systopic 'Microsoft.EventGrid/systemTopics@2022-06-15' existing = if (eventGrid.topicType == 'system') {
  name: eventGrid.topicName
}

resource custopic 'Microsoft.EventGrid/topics@2022-06-15' existing = if (eventGrid.topicType == 'custom') {
  name: eventGrid.topicName
}

resource evtgrd 'Microsoft.EventGrid/eventSubscriptions@2022-06-15' = {
  name: eventGrid.subName
  scope: eventGridTopicType == 'system' ? systopic : custopic
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
