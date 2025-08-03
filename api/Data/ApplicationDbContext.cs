using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {

        public ApplicationDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {

        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Portfolio>(x => x.HasKey(p => new { p.AppUserId, p.StockId }));

            modelBuilder.Entity<Portfolio>()
                        .HasOne(u => u.AppUser)
                        .WithMany(u => u.Portfolios)
                        .HasForeignKey(p => p.AppUserId);

            modelBuilder.Entity<Portfolio>()
                        .HasOne(u => u.Stock)
                        .WithMany(u => u.Portfolios)
                        .HasForeignKey(p => p.StockId);
            // Seed Users
            List<IdentityRole> roles =
            [
                new IdentityRole
                {
                    Id = "99D65A69-7C7C-440F-BC9F-E2A19EAF35C0",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "A3569725-34BA-49F4-A041-F5E6A76B0735",
                    Name = "User",
                    NormalizedName = "USER"
                },
            ];

            modelBuilder.Entity<IdentityRole>().HasData(roles);

            //// Seed Stocks
            //modelBuilder.Entity<Stock>().HasData(
            //    new Stock
            //    {
            //        Id = 1,
            //        Symbol = "AAPL",
            //        CompanyName = "Apple Inc.",
            //        Price = 195.34m,
            //        LastDiv = 0.24m,
            //        Industry = "Technology",
            //        MarketCap = 3000000000000
            //    },
            //    new Stock
            //    {
            //        Id = 2,
            //        Symbol = "MSFT",
            //        CompanyName = "Microsoft Corporation",
            //        Price = 410.22m,
            //        LastDiv = 0.68m,
            //        Industry = "Technology",
            //        MarketCap = 2800000000000
            //    },
            //    new Stock
            //    {
            //        Id = 3,
            //        Symbol = "TSLA",
            //        CompanyName = "Tesla, Inc.",
            //        Price = 250.12m,
            //        LastDiv = 0.00m,
            //        Industry = "Automotive",
            //        MarketCap = 800000000000
            //    }
            //);

            //// Seed Comments
            //modelBuilder.Entity<Comment>().HasData(
            //    new Comment
            //    {
            //        Id = 1,
            //        Title = "Great stock!",
            //        Content = "Apple has been performing really well this quarter.",
            //        CreatedOn = new DateTime(2024, 7, 1),
            //        StockId = 1
            //    },
            //    new Comment
            //    {
            //        Id = 2,
            //        Title = "Solid dividends",
            //        Content = "Microsoft's dividend is very attractive for long-term investors.",
            //        CreatedOn = new DateTime(2024, 7, 2),
            //        StockId = 2
            //    },
            //    new Comment
            //    {
            //        Id = 3,
            //        Title = "Volatile",
            //        Content = "Tesla's price swings a lot, but the growth is impressive.",
            //        CreatedOn = new DateTime(2024, 7, 3),
            //        StockId = 3
            //    }
            //);
        }
    }
}
