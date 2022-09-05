targetScope = 'subscription'

param name string
param location string = 'Korea Central'

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
        gitHubBranchName: gitHubBranchName
    }
}
