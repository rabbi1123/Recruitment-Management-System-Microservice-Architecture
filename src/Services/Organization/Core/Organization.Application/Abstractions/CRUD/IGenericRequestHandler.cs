using MediatR;

namespace Organization.Application.Abstractions.CRUD
{
    public interface IGenericRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IGenericRequest<TResponse>
    {
    }
}
