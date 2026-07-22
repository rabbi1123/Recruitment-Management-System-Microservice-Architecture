using Recruitment.Application.Abstractions.Files;

namespace Recruitment.WebAPI.Adapters
{
	internal sealed class FormFileIncomingFile : IIncomingFile
	{
		private readonly IFormFile _file;
		public FormFileIncomingFile(IFormFile file) => _file = file;

		public string FileName => _file.FileName;
		public long Length => _file.Length;
		public Stream OpenReadStream() => _file.OpenReadStream();
	}
}
