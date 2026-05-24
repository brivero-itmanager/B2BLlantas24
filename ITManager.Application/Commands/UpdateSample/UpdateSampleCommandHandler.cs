using ErrorOr;
using ITManager.Domain.Entities;
using ITManager.Domain.Interfaces;

namespace ITManager.Application.Commands.UpdateSample
{
    public class UpdateSampleCommandHandler
    {
        private readonly IRepository<SampleEntity> _repository;

        public UpdateSampleCommandHandler(IRepository<SampleEntity> repository)
        {
            _repository = repository;
        }

        public async Task<ErrorOr<Updated>> HandleAsync(UpdateSampleCommand command)
        {
            var entity = await _repository.GetByIdAsync(command.Id);
            if (entity is null)
            {
                return Error.NotFound("Sample.NotFound", "La entidad no existe");
            }

            entity.Update(command.Name, command.Description, command.Category, command.IsActive);
            await _repository.UpdateAsync(entity);

            return Result.Updated;
        }
    }
}
