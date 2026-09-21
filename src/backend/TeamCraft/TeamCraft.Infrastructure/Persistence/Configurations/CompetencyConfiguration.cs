using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Infrastructure.Persistence.Configurations
{
    public class CompetencyConfiguration : IEntityTypeConfiguration<Competency>
    {
        public void Configure(EntityTypeBuilder<Competency> builder) 
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Type)
                .IsRequired()
                .HasConversion<string>();

            builder.HasIndex(c => c.Name)
                .IsUnique();
        }
    }
}
