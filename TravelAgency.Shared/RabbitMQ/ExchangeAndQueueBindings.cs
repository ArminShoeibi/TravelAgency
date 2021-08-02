using RabbitMQ.Client;

namespace TravelAgency.Shared.RabbitMQ
{
    public static class ExchangeAndQueueBindings
    {
        public static void AddExchangeAndQueueForTicketStatus(this IModel amqpChannel)
        {
            /* Declaring 'Queue' and 'Exchange' is idempotent - it will only be created if it doesn't exist already 
               The declaration will have no effect if the queue/exchange does already exist and
               its attributes are the same as those in the declaration.
            */
            string ticketStatusExchange = ExchangeName.TicketStatusRequest.ToString();
            string ticketStatusQueue = ExchangeName.TicketStatusRequest.ToString();
            amqpChannel.ExchangeDeclare(ticketStatusExchange, ExchangeType.Direct, true, false);
            amqpChannel.QueueDeclare(ticketStatusQueue, true, false, false);
            amqpChannel.QueueBind(ticketStatusQueue, ticketStatusExchange, "");
        }
    }
}
