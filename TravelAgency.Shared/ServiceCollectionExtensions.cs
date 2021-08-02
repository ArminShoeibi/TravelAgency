using System;
using RabbitMQ.Client;

/// <summary>
/// We recommended that apps follow the naming convention of creating extension methods in the Microsoft.Extensions.DependencyInjection namespace.
/// Creating extension methods in the Microsoft.Extensions.DependencyInjection namespace:
///  1) Encapsulates groups of service registrations.
///  2)Provides convenient IntelliSense access to the service.
/// </summary>
namespace Microsoft.Extensions.DependencyInjection
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
