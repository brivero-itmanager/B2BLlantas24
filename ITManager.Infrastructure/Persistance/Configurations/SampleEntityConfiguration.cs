using ITManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITManager.Infrastructure.Persistance.Configurations
{
    public class SampleEntityConfiguration : IEntityTypeConfiguration<SampleEntity>
    {
        public void Configure(EntityTypeBuilder<SampleEntity> builder)
        {
            builder.ToTable("Samples");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired();

            builder.Property(x => x.Name)
                .UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction)
                .IsRequired(false)
                .HasMaxLength(300);

            builder.Property(x => x.Category)
                .UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.IsActive)
                .UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction)
                .IsRequired();
        }
    }
}
