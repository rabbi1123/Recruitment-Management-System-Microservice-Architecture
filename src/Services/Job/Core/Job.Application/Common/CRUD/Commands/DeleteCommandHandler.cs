using Job.Application.Abstractions.CRUD;
using Job.Application.Abstractions.Data;
using Common.Platform.Domain.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Job.Application.Common.CRUD.Commands
{
    public class DeleteCommandHandler<TDeleteCommand, TEntity> : IGenericRequestHandler<TDeleteCommand, Result<CommandResponse>>
        where TEntity : IEntity
        where TDeleteCommand : DeleteCommand<TEntity>
    {
        private readonly IGenericRepository<TEntity> _rCommonitory;

        public DeleteCommandHandler(
            IGenericRepository<TEntity> RCommonitory)
        {
            _rCommonitory = RCommonitory;
        }

        public async Task<Result<CommandResponse>> Handle(TDeleteCommand request, CancellationToken cancellationToken)
        {
            var affectedRows = await _rCommonitory.DeleteAsync(request.Id);

            if (affectedRows == 0)
            {
                return Result.Failure<CommandResponse>(HttpResponseStatusCodes.NotFound, Error.NotFound(typeof(TEntity).Name));
            }

            var response = new CommandResponse() { IsSuccess = true, Id = request.Id };

            return response;
        }
    }
}
