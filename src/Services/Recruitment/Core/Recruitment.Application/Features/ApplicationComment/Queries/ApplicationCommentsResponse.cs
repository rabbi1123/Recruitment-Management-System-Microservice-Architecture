using Recruitment.Application.Common.CRUD.Queries;

namespace Recruitment.Application.Features.ApplicationComment.Queries
{
	public class ApplicationCommentsResponse : IGenericResponse
	{
		public long Id { get; set; }
		public long ApplicationId { get; set; }
		public long AuthorId { get; set; }
		public string Comment { get; set; }
		public bool IsActive { get; set; }
	}
}
