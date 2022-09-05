namespace EventPublisher.Configs
{
    /// <summary>
    /// This represents the app settings entity for Event Grid.
    /// </summary>
    public class EventGridSettings
    {
        public const string Name = "EventGrid";

        /// <summary>
        /// Gets or sets the <see cref="EventGridTopicSettings"/> instance.
        /// </summary>
        public virtual EventGridTopicSettings Topic { get; set; } = new EventGridTopicSettings();
    }

    /// <summary>
    /// This represents the app settings entity for Event Grid Topic.
    /// </summary>
    public class EventGridTopicSettings
    {
        /// <summary>
        /// Gets or sets the Event Grid Topic endpoint.
        /// </summary>
        public virtual string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the Event Grid Topic access key.
        /// </summary>
        public virtual string AccessKey { get; set; }
    }
}