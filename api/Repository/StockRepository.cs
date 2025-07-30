using api.Data;
using api.DTOs.Stock;
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

        public async Task<List<Stock>> GetAllAsync()
        {
            return await context.Stocks.ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await context.Stocks.FindAsync(id);
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
