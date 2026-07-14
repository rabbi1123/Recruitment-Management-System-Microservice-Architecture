using Job.Application.Abstractions.CRUD;
using Common.Platform.Domain.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Job.Application.Common.CRUD.Queries
{
    public record GetByIdQuery<TResponse> : IGenericRequest<Result<TResponse>>
    {
        [Required(ErrorMessage = "id is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "id must be greater than 0.")]
        public long Id { get; set; }
    }
}
