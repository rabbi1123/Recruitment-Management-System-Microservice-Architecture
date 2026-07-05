namespace Candidate.Application.Abstractions.Mapping
{
    public interface IResponseEntityMapper<TEntity, TResponse>
    {
        TResponse MapToResponse(TEntity entity);
    }
}
