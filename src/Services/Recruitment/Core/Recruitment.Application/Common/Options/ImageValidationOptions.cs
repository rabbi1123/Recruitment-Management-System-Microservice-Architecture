using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recruitment.Application.Common.Options
{
    public sealed class ImageValidationOptions
    {
        [Required]
        [Range(1, long.MaxValue)]
        public long MaxBytes { get; init; }

        [Required]
        public List<string> AllowedExtensions { get; init; }

        [Required]
        public List<string> AllowedMime { get; init; }
    }
}
