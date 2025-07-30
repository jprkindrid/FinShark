using api.Data;
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
        public Task<List<Stock>> GetAllAsync()
        {
            return context.Stocks.ToListAsync();
        }
    }
}
