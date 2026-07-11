using MediatR;

namespace Organization.Application.Abstractions.CRUD
{
    public interface IGenericRequest<TResponse> : IRequest<TResponse>;
}
