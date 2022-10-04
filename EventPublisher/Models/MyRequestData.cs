using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;

using Newtonsoft.Json.Serialization;

namespace EventPublisher.Models
{
    public class MyRequestData
    {
        public string Lorem { get; set; }
    }

    public class MyRequestDataExample : OpenApiExample<MyRequestData>
    {
        public override IOpenApiExample<MyRequestData> Build(NamingStrategy namingStrategy = null)
        {
            var example = new MyRequestData()
            {
                Lorem = "Ipsum"
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