using MediatR;

namespace Recruitment.Application.Abstractions.CRUD
{
    public interface IGenericRequest<TResponse> : IRequest<TResponse>;
}
