using Candidate.Domain.Abstractions;
using System.Collections.Generic;

namespace Candidate.Application.Abstractions.DomainEvents
{
    public interface IDomainEventDispatcher
    {
        void Enqueue(IEnumerable<IDomainEvent> events);
    }
}
