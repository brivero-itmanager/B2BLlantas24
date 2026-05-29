using ITManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITManager.Infrastructure.Persistance.Configurations
{
    public class TareaLogConfiguration : IEntityTypeConfiguration<TareaLog>
    {
        public void Configure(EntityTypeBuilder<TareaLog> builder)
        {
            builder.ToTable("TareaLogs");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.TareaId)
                .IsRequired();

            builder.Property(x => x.Evento)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Detalle)
                .IsRequired(false)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.OcurridoEn)
                .IsRequired();

            builder.HasOne<Tarea>()
                .WithMany("_logs")
                .HasForeignKey(x => x.TareaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
