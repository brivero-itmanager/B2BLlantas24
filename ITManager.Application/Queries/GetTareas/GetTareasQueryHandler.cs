using ITManager.Domain.Interfaces;

namespace ITManager.Application.Queries.GetTareas
{
    public class GetTareasQueryHandler(ITareaRepository repository)
    {
        public async Task<List<TareaDto>> HandleAsync(GetTareasQuery query)
        {
            var tareas = await repository.GetAllAsync();

            return tareas.Select(tarea => new TareaDto
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
            }).ToList();
        }
    }
}
