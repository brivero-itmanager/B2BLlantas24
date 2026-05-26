using ITManager.Domain.Entities;

namespace ITManager.Domain.Interfaces
{
    public interface ITareaRepository
    {
        Task<Tarea?> GetByIdAsync(long id);
        Task<List<Tarea>> GetAllAsync();
        Task AddAsync(Tarea tarea);
        Task UpdateAsync(Tarea tarea);
    }
}
