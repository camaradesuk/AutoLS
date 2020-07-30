using System;
using SyRF.WebHostConfig.Common.Model;

namespace SyRF.BiorxivParser.Endpoint
{
    public class MessageBusConfig
    {
        public RabbitMqConfig RabbitMqConfig { get; set; }
        public string BiorxivSearchQueueName { get; set; }
        public string BiorxivParserQueueName { get; set; }

        private Uri UriFromQueue(string queueName) =>
            new Uri($"{RabbitMqConfig.ConnectionUrl}?bind=true&queue{queueName}");
        
        public Uri LiteratureSearchUri =>
            UriFromQueue(BiorxivSearchQueueName);

        public Uri BiorxivParserQueueUri =>
            UriFromQueue(BiorxivParserQueueName);
    }
}