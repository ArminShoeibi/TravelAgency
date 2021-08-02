using System;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TravelAgency.ProtocolBuffers;
using TravelAgency.Shared.RabbitMQ;

namespace TravelAgency.Providers.IranAir
{
    internal class TicketStatusRequestConsumer : AsyncEventingBasicConsumer
    {
        private readonly ILogger<TicketStatusRequestConsumer> _logger;

        public TicketStatusRequestConsumer(IModel amqpChannel,
                                           ILogger<TicketStatusRequestConsumer> logger) : base(amqpChannel)
        {
            Model = amqpChannel;
            _logger = logger;
            Model.AddExchangeAndQueueForTicketStatus();
        }

        public override async Task HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            TicketStatusRequest ticketStatusRequest = TicketStatusRequest.Parser.ParseFrom(body.ToArray());
            string ticketStatusRequestAsJson = JsonSerializer.Serialize(ticketStatusRequest);
            _logger.LogTrace(ticketStatusRequestAsJson);

            TicketStatusResponse ticketStatusResponse = new()
            {
                TicketStatusRequest = ticketStatusRequest,
                DateCreated = Timestamp.FromDateTimeOffset(DateTimeOffset.Now),
                TicketStatusCategory = TicketStatusCategory.OpenForUse,
            };
            string ticketStatusResponseAsJson = JsonSerializer.Serialize(ticketStatusResponse);
            _logger.LogInformation($"Direct Replying-To: {properties.ReplyTo} {Environment.NewLine} {ticketStatusResponseAsJson}");

            byte[] ticketStatusResponseBytes = ticketStatusResponse.ToByteArray();
            Model.BasicPublish("", properties.ReplyTo, null, ticketStatusResponseBytes);
        }
    }
}
