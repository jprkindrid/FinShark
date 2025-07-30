using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions dbContextOptions)
            :base(dbContextOptions)
        {
            
        }

        public DbSet<Models.Stock> Stocks { get; set; } 
        public DbSet<Models.Comment> Comments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Stock>().HasData(
                new Stock
                {
                    Id = 1,
                    Symbol = "AAPL",
                    CompanyName = "Apple Inc.",
                    Price = 195.34m,
                    LastDiv = 0.24m,
                    Industry = "Technology",
                    MarketCap = 3000000000000
                },
                new Stock
                {
                    Id = 2,
                    Symbol = "MSFT",
                    CompanyName = "Microsoft Corporation",
                    Price = 410.22m,
                    LastDiv = 0.68m,
                    Industry = "Technology",
                    MarketCap = 2800000000000
                },
                new Stock
                {
                    Id = 3,
                    Symbol = "TSLA",
                    CompanyName = "Tesla, Inc.",
                    Price = 250.12m,
                    LastDiv = 0.00m,
                    Industry = "Automotive",
                    MarketCap = 800000000000
                }
            );
        }
    }
}
