using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using TravelAgency.Shared;
using TravelAgency.Shared.RabbitMQ;

namespace TravelAgency.Providers.IranAir
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            IServiceProvider serviceProvider = host.Services;

            IConnection amqpConnection = serviceProvider.GetRequiredService<IConnection>();

            /* I create a new channel per consumer, I don't want one Channel for all of my consumers.
               So in consumer apps such as Consoles or Worker services you should have these 3 lines of codes per consumer.
            */
            IModel iranAirTicketStatusChannel = amqpConnection.CreateModel();
            var IranAirTicketStatusRequestConsumer = 
                ActivatorUtilities.CreateInstance<TicketStatusRequestConsumer>(serviceProvider, iranAirTicketStatusChannel);
            iranAirTicketStatusChannel.BasicConsume(QueueName.TicketStatusRequest.ToString(), true, IranAirTicketStatusRequestConsumer);


            await host.RunAsync();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddRabbitMQConnection();
                });
    }
}
