using ITManager.Domain.Entities;

namespace ITManager.Domain.Interfaces
{
    public interface ITareaRepository
    {
        Task<Tarea?> GetByIdAsync(long id);
        Task<List<Tarea>> GetAllAsync(
            string? status,
            string? uen,
            DateTime? desde,
            DateTime? hasta,
            int page,
            int pageSize);
        Task AddAsync(Tarea tarea);
        Task UpdateAsync(Tarea tarea);
        Task<List<Tarea>> GetPendingBatchAsync(int batchSize);
        Task<bool> ExisteVersionMasRecienteAsync(long tareaId, string deduplicationKey);
        Task UpdateRangeAsync(IEnumerable<Tarea> tareas);
    }
}
