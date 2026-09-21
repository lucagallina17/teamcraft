using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Infrastructure.Persistence.Configurations
{
    public class TeamAggregateConfiguration : IEntityTypeConfiguration<TeamAggregate>
    {
        public void Configure(EntityTypeBuilder<TeamAggregate> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Score)
                .IsRequired();

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.HasOne(t => t.Project)
                .WithMany(p => p.Teams)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.TeamMembers)
                .WithOne(tm => tm.Team)
                .HasForeignKey(tm => tm.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(nameof(TeamAggregate.TeamMembers))
                .HasField("_teamMembers")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
