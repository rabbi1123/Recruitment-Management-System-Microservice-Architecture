namespace Job.Infrastructure.Abstractions
{
	public interface IBackgroundEventQueue
	{
		ValueTask EnqueueAsync(Func<IServiceProvider, CancellationToken, Task> workItem);
		ValueTask<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(
			CancellationToken cancellationToken);
	}
}
