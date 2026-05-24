using ITManager.Domain.Entities;
using ITManager.Domain.Interfaces;
using ErrorOr;

namespace ITManager.Application.Queries.GetSamples
{
    public class GetSamplesQueryHandler
    {
        private readonly IRepository<SampleEntity> _repository;

        public GetSamplesQueryHandler(IRepository<SampleEntity> repository)
        {
            _repository = repository;
        }

        public async Task<ErrorOr<IEnumerable<SampleDto>>> HandleAsync(GetSamplesQuery query)
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => new SampleDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Category = e.Category,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt
            }).ToList();
        }
    }
}
