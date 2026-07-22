using Recruitment.Domain.Abstractions;

namespace Recruitment.Application.Abstractions.DomainEvents
{
	public interface IDomainEventDispatcher
	{
		void Enqueue(IEnumerable<IDomainEvent> events);
	}
}
