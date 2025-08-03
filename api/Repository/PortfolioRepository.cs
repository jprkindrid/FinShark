using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<PortfolioRepository> logger;

        public PortfolioRepository(ApplicationDbContext context, ILogger<PortfolioRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<Portfolio> CreateAsync(Portfolio portfolio)
        {
            await context.Portfolios.AddAsync(portfolio);
            await context.SaveChangesAsync();
            return portfolio;
        }

        public async Task<Portfolio?> DeletePortfolio(AppUser appUser, string symbol)
        {
            var portfolio = await context.Portfolios
                .FirstOrDefaultAsync(p =>
                    p.AppUserId == appUser.Id &&
                    p.Stock.Symbol == symbol);

            if (portfolio is null)
                return null;

            context.Portfolios.Remove(portfolio);
            await context.SaveChangesAsync();

            return portfolio;
        }

        public async Task<List<Stock>> GetUserPortfolio(AppUser user)
        {
            return await context.Portfolios.Where(u => u.AppUserId == user.Id)
                .Select(stock => new Stock
                {
                    Id = stock.StockId,
                    Symbol = stock.Stock.Symbol,
                    CompanyName = stock.Stock.CompanyName,
                    Price = stock.Stock.Price,
                    LastDiv = stock.Stock.LastDiv,
                    Industry = stock.Stock.Industry,
                    MarketCap = stock.Stock.MarketCap

                }).ToListAsync();
        }
    }
}
