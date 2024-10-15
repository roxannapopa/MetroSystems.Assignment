using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SalesAPI.Domain.Entities;

namespace SalesAPI.Data
{
    public class SalesApiDbContext : DbContext
    {
        public SalesApiDbContext(DbContextOptions<SalesApiDbContext> options)
        : base(options)
        {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Inventory> ArticleInventories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ArticleTransaction> TransactionArticles { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
               .Property(p => p.Price)
               .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Payment>()
               .Property(p => p.Amount)
               .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<ArticleTransaction>()
                .HasKey(ta => new { ta.TransactionId, ta.ArticleId });

            modelBuilder.Entity<ArticleTransaction>()
                .HasOne(ta => ta.Transaction)
                .WithMany(t => t.TransactionArticles)
                .HasForeignKey(ta => ta.TransactionId);

            modelBuilder.Entity<ArticleTransaction>()
                .HasOne(ta => ta.Article)
                .WithMany(a => a.TransactionArticles)
                .HasForeignKey(ta => ta.ArticleId);

            modelBuilder.Entity<Transaction>()
                .HasMany(t => t.Payments)
                .WithOne(p => p.Transaction)
                .HasForeignKey(p => p.TransactionId)
                .IsRequired();

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Customer)
                .WithMany(c => c.Transactions) 
                .HasForeignKey(t => t.CustomerId)
                .IsRequired();
        }
    }

}
