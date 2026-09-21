using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Infrastructure.Persistence.Configurations
{
    public class ProjectRoleRequirementConfiguration : IEntityTypeConfiguration<ProjectRoleRequirement>
    {
        public void Configure(EntityTypeBuilder<ProjectRoleRequirement> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Quantity)
                .IsRequired();

            builder.HasOne(r => r.Project)
                .WithMany(p => p.ProjectRoleRequirements)
                .HasForeignKey(r => r.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.ProjectRole)
                .WithMany(pr => pr.ProjectRoleRequirements)
                .HasForeignKey(r => r.ProjectRoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
