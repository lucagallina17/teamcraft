using Microsoft.EntityFrameworkCore;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Competency> Competencies { get; set; }
        public DbSet<EmployeeCompetencyAssessment> EmployeeCompetencyAssessments { get; set; }
        public DbSet<EmployeeAffinity> EmployeeAffinities { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectRole> ProjectRoles { get; set; }
        public DbSet<ProjectRoleRequirement> ProjectRoleRequirements { get; set; }
        public DbSet<ProjectRoleRequirementCompetency> ProjectRoleRequirementCompetencies { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<TeamReviewAggregate> TeamReviewAggregate { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
