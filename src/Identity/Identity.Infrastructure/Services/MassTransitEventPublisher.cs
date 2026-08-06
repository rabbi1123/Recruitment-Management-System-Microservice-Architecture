using Identity.Application.Common.Interfaces;
using MassTransit;

namespace Identity.Infrastructure.Services;

public sealed class MassTransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        => _publishEndpoint.Publish(message, cancellationToken);
}
