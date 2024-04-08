using CompetitorDetails.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CompetitorDetails.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<ArticleDetail> articles { get; set; }
        public DbSet<Competitor> competitors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ArticleDetail>()
                .HasOne(competitor => competitor.Competitors)
                .WithMany(article => article.ArticleDetails)
                .HasForeignKey(key => key.BrandId);
        }
    }
}