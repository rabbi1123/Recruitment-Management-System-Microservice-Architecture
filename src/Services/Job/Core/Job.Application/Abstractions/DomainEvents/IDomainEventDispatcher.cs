using Job.Domain.Abstractions;

namespace Job.Application.Abstractions.DomainEvents
{
	public interface IDomainEventDispatcher
	{
		void Enqueue(IEnumerable<IDomainEvent> events);
	}
}
