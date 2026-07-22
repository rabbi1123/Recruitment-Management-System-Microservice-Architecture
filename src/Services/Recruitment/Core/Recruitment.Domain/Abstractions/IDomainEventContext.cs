namespace Recruitment.Domain.Abstractions
{
	public interface IDomainEventContext
	{
		void Add(IDomainEvent domainEvent);
		IReadOnlyCollection<IDomainEvent> GetAll();
		void Clear();
	}
}
