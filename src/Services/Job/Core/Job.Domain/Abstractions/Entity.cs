namespace Job.Domain.Abstractions
{
    public abstract class Entity
    {
        public void Raise(IDomainEvent domainEvent, IDomainEventContext ctx)
        {
            ctx.Add(domainEvent);
        }
    }
}
