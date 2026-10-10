using MediatR;

namespace Notification.Application.Abstractions.CRUD;

public interface IGenericRequest<TResponse> : IRequest<TResponse>;
