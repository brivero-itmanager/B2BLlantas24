using ErrorOr;
using ITManager.Application.Queries.GetSamples;
using ITManager.Domain.Entities;
using ITManager.Domain.Interfaces;

namespace ITManager.Application.Queries.GetSampleById
{
    public class GetSampleByIdQueryHandler
    {
        private readonly IRepository<SampleEntity> _repository;

        public GetSampleByIdQueryHandler(IRepository<SampleEntity> repository)
        {
            _repository = repository;
        }

        public async Task<ErrorOr<SampleDto>> HandleAsync(GetSampleByIdQuery query)
        {
            var entity = await _repository.GetByIdAsync(query.Id);
            if (entity is null)
            {
                return Error.NotFound("Sample.NotFound", "La entidad no existe");
            }

            return new SampleDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Category = entity.Category,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
