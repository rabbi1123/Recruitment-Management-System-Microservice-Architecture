using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Abstractions.DomainEvents;
using Recruitment.Domain.Abstractions;
using Recruitment.Infrastructure.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Recruitment.Infrastructure.DomainEvents
{
	public sealed class DomainEventDispatcher : IDomainEventDispatcher
	{
		private readonly IBackgroundEventQueue _queue;
		private readonly ILogger<DomainEventDispatcher> _logger;

		public DomainEventDispatcher(IBackgroundEventQueue queue, ILogger<DomainEventDispatcher> logger)
		{
			_queue = queue;
			_logger = logger;
		}

		public void Enqueue(IEnumerable<IDomainEvent> events)
		{
			foreach (var @event in events)
			{
				_queue.EnqueueAsync(async (sp, token) =>
				{
					await using var scope = sp.CreateAsyncScope();

					string dbKey = (@event as IUseDb)?.DbKey ?? DbKeys.Default;

					var uowFactory = scope.ServiceProvider.GetRequiredService<IUnitOfWorkFactory>();
					var dbSession = scope.ServiceProvider.GetRequiredService<IDbSession>();

					await using var uow = uowFactory.Create(dbKey);

					try
					{
						_logger.LogInformation("Processing Background domain event {Event}", @event.GetType().Name);

						await uow.BeginAsync(IsolationLevel.ReadCommitted, token);
						dbSession.Bind(uow);

						var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

						await mediator.Publish(@event, token);

						await uow.CommitAsync(token);

						_logger.LogInformation("Completed Background domain event {Event}", @event.GetType().Name);
					}
					catch (Exception ex)
					{
						await uow.RollbackAsync(token);
						_logger.LogError(ex, "Completed Background domain event {Event} with error: {Error}", @event.GetType().Name, ex.Message);
					}
				});
			}
		}
	}
}
