using System.Collections.Generic;

namespace Candidate.Domain.Abstractions
{
    public interface IDomainEventContext
    {
        void Add(IDomainEvent domainEvent);
        IReadOnlyCollection<IDomainEvent> GetAll();
        void Clear();
    }
}
