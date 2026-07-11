using Organization.Domain.Abstractions;

namespace Organization.Application.Abstractions.DomainEvents
{
	public interface IDomainEventDispatcher
	{
		void Enqueue(IEnumerable<IDomainEvent> events);
	}
}
