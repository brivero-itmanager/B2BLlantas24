using ITManager.Domain.Entities;
using ITManager.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITManager.Infrastructure.Persistance.Repositories
{
    public class TareaRepository(ITManagerDbContext dbContext) : ITareaRepository
    {
        public async Task<Tarea?> GetByIdAsync(long id)
        {
            return await dbContext.Tareas.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Tarea>> GetAllAsync()
        {
            return await dbContext.Tareas
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Tarea tarea)
        {
            await dbContext.Tareas.AddAsync(tarea);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tarea tarea)
        {
            dbContext.Tareas.Update(tarea);
            await dbContext.SaveChangesAsync();
        }
    }
}
