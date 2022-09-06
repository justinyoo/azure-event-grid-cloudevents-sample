targetScope = 'subscription'

param name string
param location string = 'Korea Central'

@secure()
param userPrincipalName string
param gitHubBranchName string = 'main'

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
    name: 'rg-${name}'
    location: location
}

module resources './main.bicep' = {
    name: 'Resources'
    scope: rg
    params: {
        name: name
        location: location
        userPrincipalName: userPrincipalName
        gitHubBranchName: gitHubBranchName
    }
}
