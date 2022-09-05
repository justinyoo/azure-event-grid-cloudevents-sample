using System.IO;
using System.Net;
using System.Threading.Tasks;

using Azure.Messaging;
using Azure.Messaging.EventGrid;

using EventPublisher.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace EventPublisher
{
    public class EventConverterTrigger
    {
        private readonly EventGridPublisherClient _publisher;
        private readonly ILogger<EventConverterTrigger> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventConverterTrigger"/> class.
        /// </summary>
        /// <param name="publisher"><see cref="EventGridPublisherClient"/> instance.</param>
        /// <param name="logger"><see cref="ILogger{T}"/> instance.</param>
        public EventConverterTrigger(EventGridPublisherClient publisher, ILogger<EventConverterTrigger> logger)
        {
            this._publisher = publisher.ThrowIfNullOrDefault();
            this._logger = logger.ThrowIfNullOrDefault();
        }

        /// <summary>
        /// Invokes the endpoint.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <returns>Returns <see cref="IActionResult"/> instance.</returns>
        [FunctionName(nameof(EventConverterTrigger))]
        [OpenApiOperation(operationId: "events.convert", tags: new[] { "events" }, Summary = "Convert event data and publish", Description = "This converts the payload to CloudEvents and publishes it", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(MyRequestData), Example = typeof(MyRequestDataExample), Required = true, Description = "The request payload")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Summary = "The successful operation", Description = "It represents the successful operation")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "events/convert")] HttpRequest req)
        {
            this._logger.LogInformation("C# HTTP trigger function processed a request.");

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

            await this._publisher.SendEventAsync(@event);

            return new OkResult();
        }
    }
}