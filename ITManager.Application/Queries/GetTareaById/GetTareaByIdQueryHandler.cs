using ITManager.Application.Queries.GetTareas;
using ITManager.Domain.Interfaces;

namespace ITManager.Application.Queries.GetTareaById
{
    public class GetTareaByIdQueryHandler(ITareaRepository repository)
    {
        public async Task<TareaDto> HandleAsync(GetTareaByIdQuery query)
        {
            var tarea = await repository.GetByIdAsync(query.Id);
            if (tarea is null)
            {
                throw new KeyNotFoundException($"No se encontró la tarea con Id {query.Id}.");
            }

            return new TareaDto
            {
                Id = tarea.Id,
                CreatedAt = tarea.CreatedAt,
                Uen = tarea.Uen,
                NombreTarea = tarea.NombreTarea,
                Json = tarea.Json,
                Status = tarea.Status,
                Attempts = tarea.Attempts,
                LastError = tarea.LastError,
                ProcessedAt = tarea.ProcessedAt
            };
        }
    }
}
