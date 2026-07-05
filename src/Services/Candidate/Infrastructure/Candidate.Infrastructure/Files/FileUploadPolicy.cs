using Candidate.Application.Abstractions.Files;
using Candidate.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace Candidate.Infrastructure.Files
{
	public sealed class FileUploadPolicy : IFileUploadPolicy
	{
		private readonly string _root;
		private readonly long _maxBytes;
		private readonly List<string> _allowedExtensions;
		private readonly List<string> _allowedMime;

		public FileUploadPolicy(IOptions<FileStorageOptions> opt)
		{
			if (opt?.Value?.Root is null) throw new ArgumentNullException(nameof(opt));
			_maxBytes = opt.Value.MaxBytes;
			_root = opt.Value.Root;
			_allowedExtensions = opt.Value.AllowedExtensions;
		}

		public string Root => _root;
		public long MaxBytes => _maxBytes;

		public List<string> AllowedExtensions => _allowedExtensions;
	}
}
