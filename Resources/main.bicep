param name string
param location string = resourceGroup().location

param gitHubBranchName string = 'main'

module st './storageAccount.bicep' = {
  name: 'StorageAccount'
  params: {
    name: name
    location: location
  }
}

module logapp 'logicApp.bicep' = {
  name: 'LogicApp'
  params: {
    name: name
    location: location
  }
}

module kv './keyVault.bicep' = {
  name: 'KeyVault'
  params: {
    name: name
    location: location
  }
}

module evtgrdSysTopic './eventGridSystemTopic.bicep' = {
  name: 'EventGridSystemTopic'
  params: {
    name: name
    location: location
    resourceId: kv.outputs.id
    resourceType: 'Microsoft.KeyVault/vaults'
  }
}

module evtgrdSysSub 'eventGridSubscription.bicep' = {
  name: 'EventGridSystemSubscription'
  dependsOn: [
    evtgrdCusTopic
  ]
  params: {
    name: name
    location: location
    logicAppEndpointUrl: logapp.outputs.endpoint
    eventGridTopicName: evtgrdSysTopic.outputs.name
    eventGridTopicType: 'system'
  }
}

module evtgrdCusTopic './eventGridTopic.bicep' = {
  name: 'EventGridCustomTopic'
  params: {
    name: name
    location: location
  }
}

module evtgrdCusSub 'eventGridSubscription.bicep' = {
  name: 'EventGridCustomSubscription'
  dependsOn: [
    evtgrdCusTopic
  ]
  params: {
    name: name
    location: location
    logicAppEndpointUrl: logapp.outputs.endpoint
    eventGridTopicName: evtgrdCusTopic.outputs.name
    eventGridTopicType: 'custom'
  }
}

module wrkspc './logAnalyticsWorkspace.bicep' = {
  name: 'LogAnalyticsWorkspace'
  params: {
    name: name
    location: location
  }
}

module appins './appInsights.bicep' = {
  name: 'ApplicationInsights'
  params: {
    name: name
    location: location
    workspaceId: wrkspc.outputs.id
  }
}

module csplan './consumptionPlan.bicep' = {
  name: 'ConsumptionPlan'
  params: {
    name: name
    location: location
  }
}

module fncapp './functionApp.bicep' = {
  name: 'FunctionApp'
  params: {
    name: name
    location: location
    storageAccountConnectionString: st.outputs.connectionString
    appInsightsInstrumentationKey: appins.outputs.instrumentationKey
    appInsightsConnectionString: appins.outputs.connectionString
    consumptionPlanId: csplan.outputs.id
    eventGridTopicEndpoint: evtgrdCusTopic.outputs.endpoint
    eventGridTopicAccessKey: evtgrdCusTopic.outputs.accessKey
  }
}

module depscrpt './deploymentScript.bicep' = {
    name: 'DeploymentScript'
    dependsOn: [
        fncapp
    ]
    params: {
        name: name
        location: location
        gitHubBranchName: gitHubBranchName
    }
}
