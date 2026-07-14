using Job.Application.Abstractions.CRUD;
using Job.Application.Abstractions.Data;
using Job.Application.Abstractions.Mapping;
using Common.Platform.Domain.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Job.Application.Common.CRUD.Commands
{
    public class CreateCommandHandler<TCreateCommand, TEntity> : IGenericRequestHandler<TCreateCommand, Result<CommandResponse>>
        where TCreateCommand : IGenericRequest<Result<CommandResponse>>
        where TEntity : IEntity
    {
        private readonly IGenericRepository<TEntity> _rCommonitory;
        private readonly ICreateEntityMapper<TCreateCommand, TEntity> _mapper;

        public CreateCommandHandler(
            IGenericRepository<TEntity> RCommonitory,
            ICreateEntityMapper<TCreateCommand, TEntity> mapper)
        {
            _rCommonitory = RCommonitory;
            _mapper = mapper;
        }

        public async Task<Result<CommandResponse>> Handle(TCreateCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.MapCreateCommandToEntity(request);
            var id = await _rCommonitory.AddAsync(entity);

            if (id == 0)
            {
                return Result.Failure<CommandResponse>(HttpResponseStatusCodes.BadRequest, Error.EntityCouldNotBeCreated(typeof(TEntity).Name));
            }

            var response = new CommandResponse()
            {
                IsSuccess = true,
                Id = id
            };

            return response;
        }
    }

}
