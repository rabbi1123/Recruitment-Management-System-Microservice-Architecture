using Candidate.Application.Abstractions.CRUD;
using Common.Platform.Domain.Abstractions;
using System.Collections.Generic;

namespace Candidate.Application.Common.CRUD.Queries
{
    public record GetAllQuery<TResponse> : IGenericRequest<Result<List<TResponse>>>
    {
        public SortOption? SortBy { get; init; }
    }
}
