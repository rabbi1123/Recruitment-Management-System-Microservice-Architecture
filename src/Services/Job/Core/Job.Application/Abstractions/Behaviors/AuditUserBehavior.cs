using MediatR;
using Job.Application.Abstractions.Auth;
using Job.Application.Abstractions.CRUD;

namespace Job.Application.Abstractions.Behaviors
{
	public class AuditUserBehavior<TRequest, TResponse>
	: IPipelineBehavior<TRequest, TResponse>
	{
		private readonly ICurrentUserService _currentUser;

		public AuditUserBehavior(ICurrentUserService currentUser)
		{
			_currentUser = currentUser;
		}

		public async Task<TResponse> Handle(
			TRequest request,
			RequestHandlerDelegate<TResponse> next,
			CancellationToken cancellationToken)
		{
			var username = _currentUser.Username;

			if (request is ICreatedByCommand created)
				created.CreatedBy = username;

			if (request is IUpdatedByCommand updated)
				updated.UpdatedBy = username;

			if (request is IUpsertedByCommand upserted)
				upserted.UpsertedBy = username;

			return await next();
		}
	}
}
