using System;

using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EventPublisher.Models
{
    public class CloudEventData<T>
    {
        public virtual Guid Id { get; set; }

        public virtual string Source { get; set; }

        public virtual string Type { get; set; }

        public virtual T Data { get; set; }

        public virtual DateTimeOffset Time { get; set; }

        [JsonProperty("specversion")]
        public virtual string SpecVersion { get; set; } = "1.0";
    }

    public class CloudEventDataExample : OpenApiExample<CloudEventData<MyEventData>>
    {
        public override IOpenApiExample<CloudEventData<MyEventData>> Build(NamingStrategy namingStrategy = null)
        {
            var example = new CloudEventData<MyEventData>()
            {
                Id = Guid.NewGuid(),
                Source = "/api/events/publish",
                Type = "com.source.event.my/OnEventOccurs",
                Data = new MyEventData()
                {
                    Hello = "World"
                },
                Time = DateTimeOffset.UtcNow,
                SpecVersion = "1.0",
            };

            Examples.Add(
                OpenApiExampleResolver.Resolve(
                    "sample",
                    "This represents the sample entity",
                    example,
                    namingStrategy
                )
            );

            return this;
        }
    }
}