using Candidate.Application.Abstractions.Files;
using Candidate.Application.Abstractions.Images;
using Candidate.Application.Common.Options;
using Common.Platform.Domain.Abstractions;
using Microsoft.Extensions.Options;

namespace Candidate.Infrastructure.Images
{
	public sealed class ImageValidator : IImageValidator
	{
		private readonly ImageValidationOptions _opt;

		public ImageValidator(IOptions<ImageValidationOptions> options)
		{
			_opt = options.Value;
		}

		public async Task<Result<BufferedImage>> ValidateAndBufferAsync(IIncomingFile file, CancellationToken ct)
		{
			if (file is null) return Result.Failure<BufferedImage>(HttpResponseStatusCodes.BadRequest, Error.BadRequest("No file provided."));
			if (file.Length <= 0) return Result.Failure<BufferedImage>(HttpResponseStatusCodes.BadRequest, Error.BadRequest("Empty file."));
			if (file.Length > _opt.MaxBytes)
				return Result.Failure<BufferedImage>(HttpResponseStatusCodes.BadRequest, Error.BadRequest($"File too large (max {_opt.MaxBytes / (1024 * 1024)} MB)."));

			var name = file.FileName?.Trim() ?? "";

			var extFromClient = Path.GetExtension(name);
			if (string.IsNullOrWhiteSpace(extFromClient) || !_opt.AllowedExtensions.Contains(extFromClient))
				return Result.Failure<BufferedImage>(HttpResponseStatusCodes.BadRequest, Error.BadRequest($"File extension {extFromClient} is not allowed."));

			byte[] bytes;
			// Buffer once (Length already bounded)
			using (var src = file.OpenReadStream())
			{
				if (!src.CanRead) return Result.Failure<BufferedImage>(HttpResponseStatusCodes.BadRequest, Error.BadRequest("Could not read file."));
				using var ms = new MemoryStream((int)Math.Min(file.Length, int.MaxValue));
				await src.CopyToAsync(ms, ct);
				bytes = ms.ToArray();
			}

			var probeLen = Math.Min(bytes.Length, 64);
			if (probeLen < 8) return Result.Failure<BufferedImage>(HttpResponseStatusCodes.BadRequest, Error.BadRequest("File header too short."));
			var mime = DetectMimeFromHeader(bytes.AsSpan(0, probeLen));
			if (mime is null || !_opt.AllowedMime.Contains(mime))
				return Result.Failure<BufferedImage>(HttpResponseStatusCodes.BadRequest, Error.BadRequest("File signature does not match an allowed image type."));

			var ext = CanonicalExt(mime);
			return Result.Success(new BufferedImage(bytes, ext, mime));
		}

		// --- helpers ---
		private static string CanonicalExt(string mime) => mime switch
		{
			"image/jpeg" => ".jpg",
			"image/png" => ".png",
			"image/webp" => ".webp",
			_ => ".bin"
		};

		private static string? DetectMimeFromHeader(ReadOnlySpan<byte> buf)
		{
			if (buf.Length >= 3 && buf[0] == 0xFF && buf[1] == 0xD8 && buf[2] == 0xFF) return "image/jpeg"; // JPEG
			if (buf.Length >= 8 && buf[0] == 0x89 && buf[1] == 0x50 && buf[2] == 0x4E && buf[3] == 0x47 &&
							  buf[4] == 0x0D && buf[5] == 0x0A && buf[6] == 0x1A && buf[7] == 0x0A) return "image/png"; // PNG
			if (buf.Length >= 12 && buf[0] == (byte)'R' && buf[1] == (byte)'I' && buf[2] == (byte)'F' && buf[3] == (byte)'F' &&
							   buf[8] == (byte)'W' && buf[9] == (byte)'E' && buf[10] == (byte)'B' && buf[11] == (byte)'P') return "image/webp"; // WEBP
			return null;
		}
	}
}
