using Candidate.Infrastructure.Abstractions;
using System.Threading.Channels;

namespace Candidate.Infrastructure.DomainEvents
{
	public sealed class BackgroundEventQueue : IBackgroundEventQueue
	{
		private readonly Channel<Func<IServiceProvider, CancellationToken, Task>> _queue;

		public BackgroundEventQueue()
		{
			_queue = Channel.CreateUnbounded<Func<IServiceProvider, CancellationToken, Task>>(
				new UnboundedChannelOptions
				{
					SingleReader = true,
					SingleWriter = false
				});
		}

		public ValueTask EnqueueAsync(
			Func<IServiceProvider, CancellationToken, Task> workItem)
			=> _queue.Writer.WriteAsync(workItem);

		public ValueTask<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(
			CancellationToken cancellationToken)
			=> _queue.Reader.ReadAsync(cancellationToken);
	}
}
