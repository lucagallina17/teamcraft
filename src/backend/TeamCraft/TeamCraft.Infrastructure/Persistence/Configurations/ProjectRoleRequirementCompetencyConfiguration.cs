using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Infrastructure.Persistence.Configurations
{
    public class ProjectRoleRequirementCompetencyConfiguration : IEntityTypeConfiguration<ProjectRoleRequirementCompetency>
    {
        public void Configure(EntityTypeBuilder<ProjectRoleRequirementCompetency> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.MinimumLevel)
                .IsRequired();

            builder.Property(c => c.Weight)
                .IsRequired();

            builder.Property(c => c.RequirementType)
                .IsRequired()
                .HasConversion<string>();

            builder.HasOne(c => c.ProjectRoleRequirement)
                .WithMany(r => r.RequirementCompetencies)
                .HasForeignKey(c => c.ProjectRoleRequirementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Competency)
                .WithMany(c => c.ProjectRoleRequirementCompetencies)
                .HasForeignKey(c => c.CompetencyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
