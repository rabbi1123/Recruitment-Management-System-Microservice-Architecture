using Recruitment.Application.Abstractions.Mapping;
using Recruitment.Application.Features.ApplicationComment.Queries;
using Recruitment.Domain.ApplicationComment;

namespace Recruitment.Application.Features.ApplicationComment
{
	public class ApplicationCommentsMapper : IResponseEntityMapper<ApplicationComments, ApplicationCommentsResponse>
	{
		public ApplicationCommentsResponse MapToResponse(ApplicationComments entity)
		{
			return new ApplicationCommentsResponse
			{
				Id = entity.Id,
				ApplicationId = entity.ApplicationId,
				AuthorId = entity.AuthorId,
				Comment = entity.Comment,
				IsActive = entity.IsActive
			};
		}
	}
}
