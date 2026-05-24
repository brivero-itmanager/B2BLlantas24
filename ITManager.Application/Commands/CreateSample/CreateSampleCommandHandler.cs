using ITManager.Domain.Entities;
using ITManager.Domain.Interfaces;
using ErrorOr;

namespace ITManager.Application.Commands.CreateSample
{
    public class CreateSampleCommandHandler
    {
        private readonly IRepository<SampleEntity> _repository;

        public CreateSampleCommandHandler(IRepository<SampleEntity> repository)
        {
            _repository = repository;
        }

        public async Task<ErrorOr<Guid>> HandleAsync(CreateSampleCommand command)
        {
            var entity = new SampleEntity(command.Name, command.Description, command.Category, command.IsActive);
            await _repository.AddAsync(entity);
            return entity.Id;
        }
    }
}
