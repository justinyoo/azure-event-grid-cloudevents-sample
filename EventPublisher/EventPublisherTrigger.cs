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

namespace EventPublisher
{
    /// <summary>
    /// This represents the HTTP trigger entity as event publisher.
    /// </summary>
    public class EventPublisherTrigger
    {
        private readonly EventGridPublisherClient _publisher;
        private readonly ILogger<EventPublisherTrigger> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisherTrigger"/> class.
        /// </summary>
        /// <param name="publisher"><see cref="EventGridPublisherClient"/> instance.</param>
        /// <param name="logger"><see cref="ILogger{T}"/> instance.</param>
        public EventPublisherTrigger(EventGridPublisherClient publisher, ILogger<EventPublisherTrigger> logger)
        {
            this._publisher = publisher.ThrowIfNullOrDefault();
            this._logger = logger.ThrowIfNullOrDefault();
        }

        /// <summary>
        /// Invokes the endpoint.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <returns>Returns <see cref="IActionResult"/> instance.</returns>
        [FunctionName(nameof(EventPublisherTrigger))]
        [OpenApiOperation(operationId: "events.publish", tags: new[] { "events" }, Summary = "Publish event", Description = "This publishes an event in CloudEvents format", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Summary = "The successful operation", Description = "It represents the successful operation")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "events/publish")] HttpRequest req)
        {
            this._logger.LogInformation("C# HTTP trigger function processed a request.");

            var source = "/api/events/publish";
            var type = "com.source.event.my/OnEventOccurs";

            var data = new MyEventData() { Hello = "World" };

            var @event = new CloudEvent(source, type, data);

            await this._publisher.SendEventAsync(@event);

            return new OkResult();
        }
    }
}