using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Infrastructure.Persistence.Configurations
{
    public class TeamReviewAggregateConfiguration : IEntityTypeConfiguration<TeamReviewAggregate>
    {
        public void Configure(EntityTypeBuilder<TeamReviewAggregate> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Score).IsRequired();

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasOne<TeamAggregate>()
                .WithMany()
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
