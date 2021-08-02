using System;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace TravelAgency.Shared
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMQConnection(this IServiceCollection services)
        {
            ConnectionFactory amqpConnectionFactory = new()
            {
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
                Password = ConnectionFactory.DefaultPass,
                UserName = ConnectionFactory.DefaultUser,
                HostName = "localhost",
            };
            IConnection amqpConnection = amqpConnectionFactory.CreateConnection();
            services.AddSingleton(amqpConnection);
            return services;
        }
    }
}
