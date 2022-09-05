﻿using System;

using Azure;
using Azure.Messaging.EventGrid;

using EventPublisher.Configs;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Configurations.AppSettings.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(EventPublisher.Startup))]

namespace EventPublisher
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder
                   .AddEnvironmentVariables();

            base.ConfigureAppConfiguration(builder);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            ConfigureAppSettings(builder.Services);
            ConfigureClients(builder.Services);
        }

        private static void ConfigureAppSettings(IServiceCollection services)
        {
            var settings = services.BuildServiceProvider()
                                   .GetService<IConfiguration>()
                                   .Get<EventGridSettings>(EventGridSettings.Name);
            services.AddSingleton(settings);
        }

        private static void ConfigureClients(IServiceCollection services)
        {
            var settings = services.BuildServiceProvider().GetService<EventGridSettings>();
            var topicEndpoint = new Uri(settings.Topic.Endpoint);
            var credential = new AzureKeyCredential(settings.Topic.AccessKey);
            var publisher = new EventGridPublisherClient(topicEndpoint, credential);

            services.AddSingleton(publisher);
        }
    }
}