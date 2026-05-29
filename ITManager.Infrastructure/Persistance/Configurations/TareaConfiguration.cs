using ITManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITManager.Infrastructure.Persistance.Configurations
{
    public class TareaConfiguration : IEntityTypeConfiguration<Tarea>
    {
        public void Configure(EntityTypeBuilder<Tarea> builder)
        {
            builder.ToTable("Tareas");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(x => x.UpdatedAt)
                .IsRequired(false);

            builder.Property(x => x.Uen)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.NombreTarea)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("tarea");

            builder.Property(x => x.Json)
                .IsRequired()
                .HasColumnType("nvarchar(max)")
                .HasColumnName("json");

            builder.Property(x => x.Status)
                .HasMaxLength(20)
                .HasDefaultValue("pending");

            builder.Property(x => x.Attempts)
                .HasDefaultValue(0);

            builder.Property(x => x.LastError)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(x => x.ProcessedAt)
                .IsRequired(false);

            builder.Property(x => x.WooCommerceResponse)
                .IsRequired(false)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("woocommerce_response");

            builder.Property(x => x.TaskType)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("task_type");

            builder.Property(x => x.DeduplicationKey)
                .IsRequired(false)
                .HasMaxLength(100)
                .HasColumnName("deduplication_key");
        }
    }
}
