using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.RabbitMq;

public static class DependencyInjection
{
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration,
        string endpointPrefix,
        Action<IBusRegistrationConfigurator>? registerConsumers = null)
    {
        if (string.IsNullOrWhiteSpace(endpointPrefix))
            throw new ArgumentException("An endpoint prefix is required.", nameof(endpointPrefix));

        services.AddOptions<RabbitMqSettings>()
            .Bind(configuration.GetSection(RabbitMqSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var rabbitMqSettings = configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>()
            ?? new RabbitMqSettings();

        services.AddMassTransit(bus =>
        {
            registerConsumers?.Invoke(bus);

            bus.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(endpointPrefix, false));

            bus.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqSettings.Host, host =>
                {
                    host.Username(rabbitMqSettings.Username);
                    host.Password(rabbitMqSettings.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IIntegrationEventPublisher, MassTransitIntegrationEventPublisher>();

        return services;
    }
}
