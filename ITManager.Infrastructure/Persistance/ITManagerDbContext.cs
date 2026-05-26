using ITManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ITManager.Infrastructure.Persistance
{
    public class ITManagerDbContext : DbContext
    {
        public ITManagerDbContext(DbContextOptions<ITManagerDbContext> options) : base(options)
        {
        }

        public DbSet<SampleEntity> Samples => Set<SampleEntity>();
        public DbSet<Tarea> Tareas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ITManagerDbContext).Assembly);
            modelBuilder.ApplyConfiguration(new Configurations.TareaConfiguration());
        }
    }
}
