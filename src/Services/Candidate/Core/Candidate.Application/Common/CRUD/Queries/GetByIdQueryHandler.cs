using Candidate.Application.Abstractions.CRUD;
using Candidate.Application.Abstractions.Data;
using Candidate.Application.Abstractions.Mapping;
using Common.Platform.Domain.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Candidate.Application.Common.CRUD.Queries
{
    public class GetByIdQueryHandler<TEntity, TResponse>
        : IGenericRequestHandler<GetByIdQuery<TResponse>, Result<TResponse>>
        where TEntity : IEntity
        where TResponse : IGenericResponse
    {
        private readonly IGenericRepository<TEntity> _rCommonitory;
        private readonly IResponseEntityMapper<TEntity, TResponse> _mapper;

        public GetByIdQueryHandler(IGenericRepository<TEntity> RCommonitory, IResponseEntityMapper<TEntity, TResponse> mapper)
        {
            _rCommonitory = RCommonitory;
            _mapper = mapper;
        }

        public async Task<Result<TResponse>> Handle(GetByIdQuery<TResponse> request, CancellationToken cancellationToken)
        {
            var entity = await _rCommonitory.GetByIdAsync(request.Id);

            if (entity is null)
            {
                return Result.Failure<TResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound(typeof(TEntity).Name));
            }

            var response = _mapper.MapToResponse(entity);

            return response;
        }
    }
}
