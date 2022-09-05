using System;
using System.Net;
using System.Threading.Tasks;

using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EventPublisher
{
    public static class EventPublisherTrigger
    {
        [FunctionName(nameof(EventPublisherTrigger))]
        [OpenApiOperation(operationId: "events.publish", tags: new[] { "events" }, Summary = "Publish event", Description = "This publishes an event in CloudEvents format", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Summary = "The successful operation", Description = "It represents the successful operation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "events/publish")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var topicEndpoint = new Uri(Environment.GetEnvironmentVariable("EventGrid__Topic__Endpoint"));
            var credential = new AzureKeyCredential(Environment.GetEnvironmentVariable("EventGrid__Topic__AccessKey"));
            var publisher = new EventGridPublisherClient(topicEndpoint, credential);

            var source = "/api/events/publish";
            var type = "com.source.event.my/OnEventOccurs";

            var data = new MyEventData() { Hello = "World" };

            var @event = new CloudEvent(source, type, data);

            await publisher.SendEventAsync(@event);

            return new OkResult();
        }
    }
}