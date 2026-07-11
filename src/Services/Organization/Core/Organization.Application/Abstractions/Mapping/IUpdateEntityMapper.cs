namespace Organization.Application.Abstractions.Mapping
{
    public interface IUpdateEntityMapper<TUpdateCommand, TEntity>
    {
        TEntity MapUpdateCommandToEntity(TUpdateCommand command);
    }
}
