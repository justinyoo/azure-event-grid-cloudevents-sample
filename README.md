# Azure Event Grid with CloudEvents Sample #

This provides a sample [Azure Functions](https://docs.microsoft.com/azure/azure-functions/functions-overview?WT.mc_id=dotnet-75362-juyoo) app that raises events in [CloudEvents](https://cloudevents.io/) format.


## Getting Started ##

1. Click the button below to autopilot all instances and apps on Azure. Make sure that you don't forget the app name you give.

    [![Deploy To Azure](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.svg?sanitize=true)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fjustinyoo%2Fazure-event-grid-cloudevents-sample%2Fmain%2FResources%2Fazuredeploy.json)

2. Visit the following URL to check whether all the apps have been properly provisioned and deployed.

   * `https://fncapp-<AZURE_RESOURCE_NAME>.azurewebsites.net/api/swagger/ui`

3. Invoke any request on Swagger UI page.
4. Check Logic App run history whether the event has been properly captured or not.


## More Readings ##

* [#ServerlessSeptember: How to Consume CloudEvents via Azure Event Grid](https://azure.github.io/Cloud-Native/blog/to-be-announced)
* [30 Days of Serverless](https://azure.github.io/Cloud-Native/serverless-september/30DaysOfServerless)

