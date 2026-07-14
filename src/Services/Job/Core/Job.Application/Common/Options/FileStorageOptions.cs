using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Job.Application.Common.Options
{
    public class FileStorageOptions
    {
        [Required]
        public string Root { get; init; }

        [Required]
        [Range(1, long.MaxValue)]
        public long MaxBytes { get; init; }

        [Required]
        [Length(1, int.MaxValue)]
        public List<string> AllowedExtensions { get; init; }
    }
}
