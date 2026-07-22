using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Common;
using Recruitment.Domain.ApplicationComment;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Recruitment.Application.Features.ApplicationComment.Commands.AddApplicationComments
{
	public class AddApplicationCommentsCommandHandler : IRequestHandler<AddApplicationCommentsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<ApplicationComments> _repository;

		public AddApplicationCommentsCommandHandler(IGenericRepository<ApplicationComments> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(AddApplicationCommentsCommand request, CancellationToken cancellationToken)
		{
			var comment = ApplicationComments.Create(
				request.ApplicationId,
				request.AuthorId,
				request.Comment,
				request.CreatedBy);

			var affectedRows = await _repository.AddAsync(comment);

			if (affectedRows == 0)
			{
				return Result.Failure<CommandResponse>(
					HttpResponseStatusCodes.BadRequest,
					Error.EntityCouldNotBeCreated("ApplicationComments"));
			}

			return new CommandResponse { IsSuccess = true };
		}
	}
}
