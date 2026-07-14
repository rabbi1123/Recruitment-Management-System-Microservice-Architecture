using MediatR;

namespace Job.Application.Abstractions.CRUD
{
    public interface IGenericRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IGenericRequest<TResponse>
    {
    }
}
