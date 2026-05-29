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

        public async Task<List<Tarea>> GetAllAsync(
            string? status,
            string? uen,
            DateTime? desde,
            DateTime? hasta,
            int page,
            int pageSize)
        {
            var query = dbContext.Tareas.AsQueryable();

            if (status is not null)
            {
                query = query.Where(t => t.Status == status);
            }

            if (uen is not null)
            {
                query = query.Where(t => t.Uen == uen);
            }

            if (desde is not null)
            {
                query = query.Where(t => t.CreatedAt >= desde.Value);
            }

            if (hasta is not null)
            {
                query = query.Where(t => t.CreatedAt <= hasta.Value);
            }

            return await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

        public async Task<List<Tarea>> GetPendingBatchAsync(int batchSize)
        {
            return await dbContext.Tareas
                .Where(t => t.Status == "pending")
                .OrderBy(t => t.CreatedAt)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<Tarea> tareas)
        {
            dbContext.Tareas.UpdateRange(tareas);
            await dbContext.SaveChangesAsync();
        }
    }
}
