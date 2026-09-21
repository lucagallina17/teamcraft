using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Infrastructure.Persistence.Configurations
{
    public class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
    {
        public void Configure(EntityTypeBuilder<UserAccount> builder) 
        { 
            builder.HasKey(x => x.Id);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(u => u.Employee)
                .WithOne(e => e.UserAccount)
                .HasForeignKey<UserAccount>(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
