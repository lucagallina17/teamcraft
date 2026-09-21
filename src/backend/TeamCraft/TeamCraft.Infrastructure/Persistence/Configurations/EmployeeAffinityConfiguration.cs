using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Infrastructure.Persistence.Configurations
{
    public class EmployeeAffinityConfiguration : IEntityTypeConfiguration<EmployeeAffinity>
    {
        public void Configure(EntityTypeBuilder<EmployeeAffinity> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Score)
                .IsRequired();

            builder.Property(a => a.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.HasOne(a => a.Employee1)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId1)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Employee2)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId2)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
