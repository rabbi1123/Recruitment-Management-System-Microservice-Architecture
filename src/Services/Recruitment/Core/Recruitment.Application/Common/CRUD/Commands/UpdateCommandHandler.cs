using Recruitment.Application.Abstractions.CRUD;
using Recruitment.Application.Abstractions.Data;
using Recruitment.Application.Abstractions.Mapping;
using Common.Platform.Domain.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Recruitment.Application.Common.CRUD.Commands
{
    public class UpdateCommandHandler<TUpdateCommand, TEntity> : IGenericRequestHandler<TUpdateCommand, Result<CommandResponse>>
        where TUpdateCommand : IGenericRequest<Result<CommandResponse>>
        where TEntity : IEntity
    {
        private readonly IGenericRepository<TEntity> _rCommonitory;
        private readonly IUpdateEntityMapper<TUpdateCommand, TEntity> _mapper;

        public UpdateCommandHandler(
            IGenericRepository<TEntity> RCommonitory,
            IUpdateEntityMapper<TUpdateCommand, TEntity> mapper)
        {
            _rCommonitory = RCommonitory;
            _mapper = mapper;
        }

        public async Task<Result<CommandResponse>> Handle(TUpdateCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.MapUpdateCommandToEntity(request);

            var affectedRows = await _rCommonitory.UpdateAsync(entity);

            if (affectedRows == 0)
            {
                return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound(typeof(TEntity).Name));
            }

            var response = new CommandResponse() { IsSuccess = true, };

            return response;
        }
    }
}
