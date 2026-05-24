using ErrorOr;
using ITManager.Domain.Entities;
using ITManager.Domain.Interfaces;

namespace ITManager.Application.Commands.DeleteSample
{
    public class DeleteSampleCommandHandler
    {
        private readonly IRepository<SampleEntity> _repository;

        public DeleteSampleCommandHandler(IRepository<SampleEntity> repository)
        {
            _repository = repository;
        }

        public async Task<ErrorOr<Deleted>> HandleAsync(DeleteSampleCommand command)
        {
            var entity = await _repository.GetByIdAsync(command.Id);
            if (entity is null)
            {
                return Error.NotFound("Sample.NotFound", "La entidad no existe");
            }

            await _repository.DeleteAsync(entity);
            return Result.Deleted;
        }
    }
}
