using Organization.Application.Abstractions.Files;
using Organization.Application.Common.Options;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Organization.Infrastructure.Files
{
	public sealed class FileSystemStorageService : IFileStorageService
	{
		private readonly string _root;
		public FileSystemStorageService(IOptions<FileStorageOptions> opt)
		{
			if (opt?.Value?.Root is null) throw new ArgumentNullException(nameof(opt));
			_root = EnsureRoot(opt.Value.Root);
		}

		public StoredFileInfo GenerateRelativePath(string subFolder, IIncomingFile file, string? newFileName)
		{
			if (subFolder is null) throw new ArgumentNullException(nameof(subFolder));

			var relDir = NormalizeSubFolder(subFolder);

			var original = (Path.GetFileName(file.FileName ?? string.Empty) ?? string.Empty).Trim();
			var ext = Path.GetExtension(original) ?? string.Empty;

			if (string.IsNullOrWhiteSpace(newFileName)) newFileName = Guid.NewGuid().ToString("N");

			var uniqueName = $"{Slug(newFileName)}{ext.ToLowerInvariant()}";

			var relPath = Path.Combine(relDir, uniqueName).Replace('\\', '/'); // always store with '/'

			// Validate final path does not escape root
			var absPath = Path.Combine(_root, relPath.Replace('/', Path.DirectorySeparatorChar));
			EnsureUnderRoot(absPath);

			return new StoredFileInfo
			{
				RelativePath = relPath,
				FileName = uniqueName,
				Extension = ext,
				ContentType = GetMime(uniqueName),
			};
		}

		public async Task<StoredFileInfo> SaveAsync(IIncomingFile file, string relativePath, CancellationToken ct)
		{
			if (file is null) throw new ArgumentNullException(nameof(file));
			if (string.IsNullOrWhiteSpace(relativePath)) throw new ArgumentNullException(nameof(relativePath));

			var rel = relativePath.Replace('\\', '/').Trim('/');
			if (rel.Contains("..", StringComparison.Ordinal) || rel.Contains(':', StringComparison.Ordinal))
				throw new InvalidOperationException("Invalid relative path.");

			var abs = Path.Combine(_root, rel.Replace('/', Path.DirectorySeparatorChar));
			EnsureUnderRoot(abs);

			var dir = Path.GetDirectoryName(abs)!;
			Directory.CreateDirectory(dir);

			long totalBytes = 0;
			string shaHex;

			await using (var src = file.OpenReadStream())
			await using (var dst = new FileStream(
				abs,
				FileMode.CreateNew,
				FileAccess.Write,
				FileShare.None,
				bufferSize: 64 * 1024,
				options: FileOptions.Asynchronous | FileOptions.SequentialScan))
			using (var sha = SHA256.Create())
			{
				var buf = new byte[128 * 1024];
				int read;
				while ((read = await src.ReadAsync(buf.AsMemory(0, buf.Length), ct)) > 0)
				{
					await dst.WriteAsync(buf.AsMemory(0, read), ct);
					sha.TransformBlock(buf, 0, read, null, 0);
					totalBytes += read;
				}
				sha.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
				shaHex = Convert.ToHexString(sha.Hash!).ToLowerInvariant();
			}

			var ext = Path.GetExtension(rel);
			var fileNameOnly = Path.GetFileName(rel);

			return new StoredFileInfo
			{
				RelativePath = rel,
				FileName = fileNameOnly,
				Extension = ext,
				ContentType = GetMime(fileNameOnly),
			};
		}

		public Task DeleteAsync(string relativeFilePath, CancellationToken ct)
		{
			if (string.IsNullOrWhiteSpace(relativeFilePath))
				throw new ArgumentNullException(nameof(relativeFilePath));

			var rel = relativeFilePath.Replace('\\', '/').Trim('/');
			if (rel.Contains("..", StringComparison.Ordinal) || rel.Contains(':', StringComparison.Ordinal))
				throw new InvalidOperationException("Invalid relative path.");

			var abs = Path.Combine(_root, rel.Replace('/', Path.DirectorySeparatorChar));
			EnsureUnderRoot(abs);

			if (File.Exists(abs))
			{
				ct.ThrowIfCancellationRequested();
				File.Delete(abs);
			}

			return Task.CompletedTask;
		}

		public Task<Stream> OpenReadAsync(string relativePath, CancellationToken ct)
		{
			var rel = relativePath.Replace('\\', '/').Trim('/');
			if (rel.Contains("..", StringComparison.Ordinal) || rel.Contains(':', StringComparison.Ordinal))
				throw new InvalidOperationException("Invalid relative path.");

			var abs = Path.Combine(_root, rel.Replace('/', Path.DirectorySeparatorChar));
			EnsureUnderRoot(abs);

			Stream s = new FileStream(
				abs, FileMode.Open, FileAccess.Read, FileShare.Read,
				64 * 1024, FileOptions.Asynchronous | FileOptions.SequentialScan);

			return Task.FromResult(s);
		}

		// --- helpers ---
		private static string EnsureRoot(string root)
		{
			var full = Path.GetFullPath(root);
			Directory.CreateDirectory(full);
			return full;
		}

		private static string NormalizeSubFolder(string subFolder)
		{
			var clean = (subFolder ?? string.Empty).Replace('\\', '/').Trim('/');
			if (clean.Length == 0) return string.Empty;

			foreach (var part in clean.Split('/'))
			{
				if (part.Length == 0) continue;
				if (part == ".." || part.Contains(':', StringComparison.Ordinal))
					throw new InvalidOperationException("Invalid folder segment.");
			}
			return clean;
		}

		private void EnsureUnderRoot(string absolutePath)
		{
			var full = Path.GetFullPath(absolutePath);
			if (!full.StartsWith(_root, StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException("Path escapes storage root.");
		}

		private static string Slug(string name)
		{
			var s = new string(name.Where(ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '-').ToArray());
			return string.IsNullOrWhiteSpace(s) ? "file" : s.ToLowerInvariant();
		}

		public string GetMime(string? fileNameOrExt)
		{
			// Try by file name (better mapping), fallback to extension.
			var ext = Path.GetExtension(fileNameOrExt);
			return (ext ?? "").ToLowerInvariant() switch
			{
				".jpg" or ".jpeg" => "image/jpeg",
				".png" => "image/png",
				".webp" => "image/webp",
				".gif" => "image/gif",
				".pdf" => "application/pdf",
				".doc" => "application/msword",
				".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
				".xls" => "application/vnd.ms-excel",
				".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
				_ => "application/octet-stream"
			};
		}


	}
}
