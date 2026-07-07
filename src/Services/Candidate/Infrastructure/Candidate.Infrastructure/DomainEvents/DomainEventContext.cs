using Candidate.Domain.Abstractions;

namespace Candidate.Infrastructure.DomainEvents
{
	public sealed class DomainEventContext : IDomainEventContext
	{
		private readonly List<IDomainEvent> _events = new();

		public void Add(IDomainEvent domainEvent)
			=> _events.Add(domainEvent);

		public IReadOnlyCollection<IDomainEvent> GetAll()
			=> _events.AsReadOnly();

		public void Clear()
			=> _events.Clear();
	}
}
