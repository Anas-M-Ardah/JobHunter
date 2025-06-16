using JobHunter.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobHunter.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Project> Projects { get; set; }

        public DbSet<Resume> Resumes { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

           protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Portfolio -> User relationship (One User has Many Portfolios)
            modelBuilder.Entity<Portfolio>()
                .HasOne(p => p.EndUser)
                .WithMany(u => u.Portfolios)
                .HasForeignKey(p => p.EndUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Service -> Portfolio relationship
            modelBuilder.Entity<Service>()
                .HasOne<Portfolio>()
                .WithMany(p => p.Services)
                .HasForeignKey(s => s.PortfolioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Project -> Portfolio relationship
            modelBuilder.Entity<Project>()
                .HasOne<Portfolio>()
                .WithMany(p => p.Projects)
                .HasForeignKey(proj => proj.PortfolioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<EndUser>("EndUser")
                .HasValue<Admin>("Admin");
        }
    }

}
