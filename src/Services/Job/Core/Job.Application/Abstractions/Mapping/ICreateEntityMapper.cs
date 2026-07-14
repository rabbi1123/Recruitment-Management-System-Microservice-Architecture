namespace Job.Application.Abstractions.Mapping
{
    public interface ICreateEntityMapper<TCreateCommand, TEntity>
    {
        TEntity MapCreateCommandToEntity(TCreateCommand command);
    }
}
