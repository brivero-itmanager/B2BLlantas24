using ITManager.Domain.Entities;
using ITManager.Domain.Interfaces;
using ITManager.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace ITManager.Infrastructure.Repositories
{
    public class SampleRepository(ITManagerDbContext dbContext) : IRepository<SampleEntity>
    {
        public async Task<IEnumerable<SampleEntity>> GetAllAsync()
        {
            return await dbContext.Samples.ToListAsync();
        }

        public async Task<SampleEntity?> GetByIdAsync(Guid id)
        {
            return await dbContext.Samples.FindAsync(id);
        }

        public async Task AddAsync(SampleEntity entity)
        {
            await dbContext.Samples.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(SampleEntity entity)
        {
            dbContext.Samples.Update(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(SampleEntity entity)
        {
            dbContext.Samples.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}
