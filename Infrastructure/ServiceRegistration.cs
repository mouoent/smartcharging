using Infrastructure.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Shared.Interfaces;

namespace Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Event Publisher
        services.AddScoped<IEventPublisher, EventPublisher>();

        // Register HttpClient services for API clients
        services.AddHttpClient<IGroupServiceClient, GroupServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["ApiClientSettings:GroupServiceBaseUrl"] ?? string.Empty);
        });

        services.AddHttpClient<IConnectorServiceClient, ConnectorServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["ApiClientSettings:ConnectorServiceBaseUrl"] ?? string.Empty);
        });

        services.AddHttpClient<IChargeStationServiceClient, ChargeStationServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["ApiClientSettings:ChargeStationServiceBaseUrl"] ?? string.Empty);
        });

        // Register RabbitMQ connection
        services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"],
            UserName = configuration["RabbitMQ:UserName"],
            Password = configuration["RabbitMQ:Password"]
        });

        services.AddSingleton(sp => sp.GetRequiredService<IConnectionFactory>().CreateConnection());

        return services;
    }
}
