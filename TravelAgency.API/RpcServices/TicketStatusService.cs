using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TravelAgency.ProtocolBuffers;
using TravelAgency.Shared.RabbitMQ;

namespace TravelAgency.API.RpcServices
{
    public class TicketStatusService
    {
        private readonly ILogger<TicketStatusService> _logger;
        private readonly IConnection _amqpConnection;
        private readonly IModel _amqpChannel;
        private readonly IBasicProperties _basicProperties;
        private readonly AsyncEventingBasicConsumer _ticketStatusResponseConsumer;
        // private readonly BlockingCollection<TicketStatusResponse> _ticketStatusResponseQueue = new();
        // private readonly BufferBlock<TicketStatusResponse> _ticketStatusResponseQueue = new();
        private readonly Channel<TicketStatusResponse> _ticketStatusResponsechannel;


        public TicketStatusService(IConnection amqpConnection, ILogger<TicketStatusService> logger)
        {
            _ticketStatusResponsechannel = Channel.CreateUnbounded<TicketStatusResponse>();
            _logger = logger;
            _amqpConnection = amqpConnection;
            _amqpChannel = _amqpConnection.CreateModel();
            _basicProperties = _amqpChannel.CreateBasicProperties();
            _basicProperties.ReplyTo = "amq.rabbitmq.reply-to";

            _amqpChannel.AddExchangeAndQueueForTicketStatus();

            _ticketStatusResponseConsumer = new(_amqpChannel);
            _ticketStatusResponseConsumer.Received += async (sender, ea) =>
            {
                await Task.Delay(1);
                TicketStatusResponse ticketStatusResponse = TicketStatusResponse.Parser.ParseFrom(ea.Body.ToArray());
                string ticketStatusResponseAsJson = JsonSerializer.Serialize(ticketStatusResponse);
                _logger.LogInformation($"Response Received: {ticketStatusResponseAsJson}");
                await _ticketStatusResponsechannel.Writer.WriteAsync(ticketStatusResponse);
            };
            _amqpChannel.BasicConsume(_basicProperties.ReplyTo, true, _ticketStatusResponseConsumer);

        }

        public async Task<TicketStatusResponse> GetTicketStatus(TicketStatusRequest ticketStatusRequest)
        {
            var ticketStatusRequestBytes = ticketStatusRequest.ToByteArray();
            _amqpChannel.BasicPublish(ExchangeName.TicketStatusRequest.ToString(), "", _basicProperties, ticketStatusRequestBytes);
            return await _ticketStatusResponsechannel.Reader.ReadAsync();
        }
    }
}
