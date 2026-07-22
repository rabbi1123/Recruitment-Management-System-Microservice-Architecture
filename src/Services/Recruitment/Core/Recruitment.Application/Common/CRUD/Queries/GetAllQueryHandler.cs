using Recruitment.Application.Abstractions.CRUD;
using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Abstractions.Mapping;
using Common.Platform.Domain.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Recruitment.Application.Common.CRUD.Queries
{
    public class GetAllQueryHandler<TEntity, TResponse>
        : IGenericRequestHandler<GetAllQuery<TResponse>, Result<List<TResponse>>>
        where TEntity : IEntity
        where TResponse : IGenericResponse
    {
        private readonly IGenericRepository<TEntity> _rCommonitory;
        private readonly IResponseEntityMapper<TEntity, TResponse> _mapper;

        public GetAllQueryHandler(IGenericRepository<TEntity> RCommonitory, IResponseEntityMapper<TEntity, TResponse> mapper)
        {
            _rCommonitory = RCommonitory;
            _mapper = mapper;
        }

        public async Task<Result<List<TResponse>>> Handle(GetAllQuery<TResponse> request, CancellationToken cancellationToken)
        {
            var enitities = await _rCommonitory.GetAllAsync(request.SortBy?.PropertyName, request.SortBy?.SortBy);

            if (enitities is null)
            {
                return Result.Failure<List<TResponse>>(HttpResponseStatusCodes.NotFound, Error.NotFound(typeof(TEntity).Name));
            }

            var response = enitities.Select(entity => _mapper.MapToResponse(entity)).ToList();

            return response;
        }
    }
}
