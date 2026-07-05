using MediatR;

namespace Candidate.Application.Abstractions.CRUD
{
    public interface IGenericRequest<TResponse> : IRequest<TResponse>;
}
