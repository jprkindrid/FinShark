using api.Data;
using api.DTOs.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext context;

        public StockRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Stock> CreateAsync(Stock stock)
        {
            await context.Stocks.AddAsync(stock);
            await context.SaveChangesAsync();
            return stock;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stock = await context.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if (stock == null)
            {
                return null;
            }

            context.Stocks.Remove(stock);
            await context.SaveChangesAsync();
            return stock;
        }

        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {
            var stocks = context.Stocks.Include(s => s.Comments).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }

            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }
            if (!string.IsNullOrWhiteSpace(query.SortBy))
                switch (query.SortBy.ToLowerInvariant())
                {
                    case "symbol":
                        stocks = query.IsDescending 
                            ? stocks.OrderByDescending(s => s.Symbol)
                            : stocks.OrderBy(s => s.Symbol);
                        break;

                    case "companyname":
                        stocks = query.IsDescending
                            ? stocks.OrderByDescending(s => s.CompanyName)
                            : stocks.OrderBy(s => s.Symbol);
                        break;
                    case "marketcap":
                        stocks = query.IsDescending
                            ? stocks.OrderByDescending(s => s.MarketCap)
                            : stocks.OrderBy(s => s.MarketCap);
                        break;

                    case "price":
                        stocks = query.IsDescending
                            ? stocks.OrderByDescending(s => s.Price)
                            : stocks.OrderBy(s => s.Price);
                        break;

                    case "lastdiv":
                        stocks = query.IsDescending
                            ? stocks.OrderByDescending(s => s.LastDiv)
                            : stocks.OrderBy(s => s.LastDiv);
                        break;

                    case "industry":
                        stocks = query.IsDescending
                            ? stocks.OrderByDescending(s => s.Industry)
                            : stocks.OrderBy(s => s.Industry);
                        break;

                    default:
                        break;
                }

            return await stocks.ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await context.Stocks.Include(s => s.Comments).FirstOrDefaultAsync(i => i.Id == id);
        }

        public Task<bool> StockExists(int id)
        {
            return context.Stocks.AnyAsync(s => s.Id == id);
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDTO stockDTO)
        {
            var existingStock = await context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            if (existingStock == null)
            {
                return null;
            }

            existingStock.Symbol = stockDTO.Symbol;
            existingStock.CompanyName = stockDTO.CompanyName;
            existingStock.Industry = stockDTO.Industry;
            existingStock.Price = stockDTO.Price;
            existingStock.LastDiv = stockDTO.LastDiv;
            existingStock.MarketCap = stockDTO.MarketCap;

            await context.SaveChangesAsync();

            return existingStock;

        }
    }
}
