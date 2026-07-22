using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Common;
using Recruitment.Domain.ApplicationComment;
using Common.Platform.Domain.Abstractions;
using MediatR;

namespace Recruitment.Application.Features.ApplicationComment.Commands.UpdateApplicationComments
{
	public class UpdateApplicationCommentsCommandHandler : IRequestHandler<UpdateApplicationCommentsCommand, Result<CommandResponse>>
	{
		private readonly IGenericRepository<ApplicationComments> _repository;

		public UpdateApplicationCommentsCommandHandler(IGenericRepository<ApplicationComments> repository)
		{
			_repository = repository;
		}

		public async Task<Result<CommandResponse>> Handle(UpdateApplicationCommentsCommand request, CancellationToken cancellationToken)
		{
			var comment = await _repository.GetByIdAsync(request.Id);
			if (comment is not null)
			{
				comment.Update(
					request.Comment,
					request.IsActive,
					request.UpdatedBy);

				var affectedRows = await _repository.UpdateAsync(comment);

				if (affectedRows == 0)
				{
					return Result.Failure<CommandResponse>(
						HttpResponseStatusCodes.NotFound,
						Error.NotFound("ApplicationComments"));
				}

				return new CommandResponse { IsSuccess = true };
			}

			return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound("ApplicationComment"));
		}
	}
}
