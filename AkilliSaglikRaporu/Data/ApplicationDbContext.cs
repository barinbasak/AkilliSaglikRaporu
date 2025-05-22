using AkilliSaglikRaporu.Models;
using Microsoft.EntityFrameworkCore;

namespace AkilliSaglikRaporu.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<HealthReport> HealthReports { get; set; }
        public DbSet<HealthReportShare> HealthReportShares { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // HealthReport ve HealthReportShare arası ilişki
            modelBuilder.Entity<HealthReportShare>()
                .HasOne(s => s.HealthReport)
                .WithMany(r => r.Shares)
                .HasForeignKey(s => s.HealthReportId);

            base.OnModelCreating(modelBuilder);
        }
    }
} 