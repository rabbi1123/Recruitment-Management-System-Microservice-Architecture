using Candidate.Application.Features.Candidate.Commands.AddCandidates;
using Common.Platform.Domain.Abstractions;
using MassTransit;
using MediatR;
using Messaging.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace Candidate.Infrastructure.Messaging;

public sealed class EmployeeRegisteredConsumer : IConsumer<EmployeeRegisteredEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<EmployeeRegisteredConsumer> _logger;

    public EmployeeRegisteredConsumer(IMediator mediator, ILogger<EmployeeRegisteredConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EmployeeRegisteredEvent> context)
    {
        var message = context.Message;

        var result = await _mediator.Send(
            new AddCandidatesCommand
            {
                UserId = message.UserId,
                Email = message.Email,
                FirstName = "",
                LastName = "",
                IsActive = true,
                CreatedBy = "IdentityService"
            },
            context.CancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation(
                "Created candidate for registered employee {UserId} ({Email})",
                message.UserId,
                message.Email);
            return;
        }

        // Idempotent: already created from a prior delivery
        if (result.Status == (int)HttpResponseStatusCodes.Conflict)
        {
            _logger.LogInformation(
                "Candidate already exists for employee {UserId} ({Email}); acknowledging message",
                message.UserId,
                message.Email);
            return;
        }

        throw new InvalidOperationException(
            $"Failed to create candidate for {message.UserId}: {result.Error?.Message}");
    }
}
