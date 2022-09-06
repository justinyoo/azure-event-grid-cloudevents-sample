param name string
param location string = resourceGroup().location

// param userPrincipalName string
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
    resourceType: 'Microsoft.KeyVault.vaults'
  }
}

module evtgrdSysSub './eventGridSystemSubscription.bicep' = {
  name: 'EventGridSystemSubscription'
  params: {
    name: name
    location: location
    eventGridTopicName: evtgrdSysTopic.outputs.name
    logicAppEndpointUrl: logapp.outputs.endpoint
  }
}

module evtgrdTopic './eventGridTopic.bicep' = {
  name: 'EventGridTopic'
  params: {
    name: name
    location: location
  }
}

module evtgrdSub './eventGridSubscription.bicep' = {
  name: 'EventGridSubscription'
  dependsOn: [
    evtgrdTopic
  ]
  params: {
    name: name
    location: location
    eventGridTopicName: evtgrdTopic.outputs.name
    logicAppEndpointUrl: logapp.outputs.endpoint
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
    eventGridTopicEndpoint: evtgrdTopic.outputs.endpoint
    eventGridTopicAccessKey: evtgrdTopic.outputs.accessKey
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
        // userPrincipalName: userPrincipalName
        gitHubBranchName: gitHubBranchName
    }
}
