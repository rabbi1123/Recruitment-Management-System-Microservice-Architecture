using Organization.Infrastructure.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Organization.Infrastructure.DomainEvents
{
	public sealed class BackgroundDomainEventWorker : BackgroundService
	{
		private readonly IBackgroundEventQueue _queue;
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly ILogger<BackgroundDomainEventWorker> _logger;

		public BackgroundDomainEventWorker(
			IBackgroundEventQueue queue,
			IServiceScopeFactory scopeFactory,
			ILogger<BackgroundDomainEventWorker> logger)
		{
			_queue = queue;
			_scopeFactory = scopeFactory;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var workItem = await _queue.DequeueAsync(stoppingToken);

				try
				{
					await using var scope = _scopeFactory.CreateAsyncScope();
					await workItem(scope.ServiceProvider, stoppingToken);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Background domain event failed");
				}
			}
		}
	}
}
