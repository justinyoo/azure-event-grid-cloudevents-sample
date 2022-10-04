using System.IO;
using System.Net;
using System.Threading.Tasks;

using EventPublisher.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json;
using Azure.Messaging;

namespace EventPublisher
{
    public class EventHandlerTrigger
    {
        private readonly ILogger<EventConverterTrigger> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventConverterTrigger"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILogger{T}"/> instance.</param>
        public EventHandlerTrigger(ILogger<EventConverterTrigger> logger)
        {
            this._logger = logger.ThrowIfNullOrDefault();
        }

        /// <summary>
        /// Invokes the validation endpoint.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <returns>Returns <see cref="IActionResult"/> instance.</returns>
        [FunctionName(nameof(EventHandlerTrigger.Validate))]
        [OpenApiOperation(operationId: "events.validate", tags: new[] { "events" }, Summary = "Validate the origin of the request", Description = "This validates the origin of the request", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "WebHook-Request-Origin", In = ParameterLocation.Header, Required = true, Type = typeof(string), Summary = "The request origin", Description = "This indicates the origin of the request", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "WebHook-Request-Rate", In = ParameterLocation.Header, Type = typeof(string), Summary = "The request rate", Description = "This indicates the rate of the request", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Summary = "The successful operation", Description = "It represents the successful operation")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.MethodNotAllowed, Summary = "The invalid operation", Description = "It represents the invalid operation")]
        public async Task<IActionResult> Validate(
            [HttpTrigger(AuthorizationLevel.Function, "OPTIONS", Route = "events/handle")] HttpRequest req)
        {
            this._logger.LogInformation("C# HTTP trigger function processed a request.");

            var origin = req.Headers["WebHook-Request-Origin"];
            if ((string)origin != "eventgrid.azure.net")
            {
                return new StatusCodeResult((int)HttpStatusCode.MethodNotAllowed);
            }

            req.HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", origin);

            var rate = req.Headers["WebHook-Request-Rate"];
            if (((string)rate).IsNullOrWhiteSpace() == false)
            {
                req.HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", rate);
            }

            var result = new OkResult();
            return await Task.FromResult(result).ConfigureAwait(false);
        }

        /// <summary>
        /// Invokes the event handling endpoint.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <returns>Returns <see cref="IActionResult"/> instance.</returns>
        [FunctionName(nameof(EventHandlerTrigger.Handle))]
        [OpenApiOperation(operationId: "events.handle", tags: new[] { "events" }, Summary = "Handle event data", Description = "This handles the payload", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CloudEventData<MyEventData>), Example = typeof(CloudEventDataExample), Required = true, Description = "The request payload")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Accepted, Summary = "The successful operation", Description = "It represents the successful operation")]
        public async Task<IActionResult> Handle(
            [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "events/handle")] HttpRequest req)
        {
            this._logger.LogInformation("C# HTTP trigger function processed a request.");

            var data = default(CloudEventData<MyEventData>);
            using (var reader = new StreamReader(req.Body))
            {
                var serialised = await reader.ReadToEndAsync().ConfigureAwait(false);
                data = JsonConvert.DeserializeObject<CloudEventData<MyEventData>>(serialised);
            }

            // CALL TARGET SYSTEM HERE WITH THE DATA

            return new AcceptedResult();
        }
    }
}