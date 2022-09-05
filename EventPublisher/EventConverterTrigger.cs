using System;
using System.IO;
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

using Newtonsoft.Json;

namespace EventPublisher
{
    public static class EventConverterTrigger
    {
        [FunctionName(nameof(EventConverterTrigger))]
        [OpenApiOperation(operationId: "events.convert", tags: new[] { "events" }, Summary = "Convert event data and publish", Description = "This converts the payload to CloudEvents and publishes it", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(MyRequestData), Example = typeof(MyRequestDataExample), Required = true, Description = "The request payload")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Summary = "The successful operation", Description = "It represents the successful operation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "events/convert")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var topicEndpoint = new Uri(Environment.GetEnvironmentVariable("EventGrid__Topic__Endpoint"));
            var credential = new AzureKeyCredential(Environment.GetEnvironmentVariable("EventGrid__Topic__AccessKey"));
            var publisher = new EventGridPublisherClient(topicEndpoint, credential);

            var source = "/api/events/convert";
            var type = "com.source.event.my/OnEventOccurs";

            var data = default(MyRequestData);
            using (var reader = new StreamReader(req.Body))
            {
                var serialised = await reader.ReadToEndAsync();
                data = JsonConvert.DeserializeObject<MyRequestData>(serialised);
            }

            var converted = new MyEventData() { Hello = data.Lorem };
            var @event = new CloudEvent(source, type, converted);

            await publisher.SendEventAsync(@event);

            return new OkResult();
        }
    }
}