using System;
using SyRF.WebHostConfig.Common.Model;

namespace SyRF.LivingSearch.Endpoint
{
    public class MessageBusConfig
    {
        public RabbitMqConfig RabbitMqConfig { get; set; }
        public string PubmedSearchQueueName { get; set; }
        public string PubmedParserQueueName { get; set; }

        private Uri UriFromQueue(string queueName) =>
            new Uri($"{RabbitMqConfig.ConnectionUrl}/{queueName}?bind=true&queue{queueName}");
        public Uri LiteratureSearchUri =>
            UriFromQueue(PubmedSearchQueueName);

        public Uri PubmedParserQueueUri =>
            UriFromQueue(PubmedParserQueueName);
    }
}