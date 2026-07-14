using MediatR;

namespace Job.Application.Abstractions.CRUD
{
    public interface IGenericRequest<TResponse> : IRequest<TResponse>;
}
