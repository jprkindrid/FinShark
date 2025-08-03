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
        private readonly ILogger<StockRepository> logger;

        public StockRepository(ApplicationDbContext context, ILogger<StockRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<Stock> CreateAsync(Stock stock)
        {
            try
            {
                await context.Stocks.AddAsync(stock);
                await context.SaveChangesAsync();
                logger.LogInformation("Successfully created stock with symbol: {Symbol}", stock.Symbol);
                return stock;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating stock with symbol: {Symbol}", stock.Symbol);
                throw;
            }
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            try
            {
                var stock = await context.Stocks.FirstOrDefaultAsync(x => x.Id == id);
                if (stock == null)
                {
                    logger.LogWarning("Attempted to delete non-existent stock with id: {Id}", id);
                    return null;
                }

                context.Stocks.Remove(stock);
                await context.SaveChangesAsync();
                logger.LogInformation("Successfully deleted stock with id: {Id}, symbol: {Symbol}", id, stock.Symbol);
                return stock;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting stock with id: {Id}", id);
                throw;
            }
        }

        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {
            try
            {
                var stocks = context.Stocks.Include(s => s.Comments).ThenInclude(a => a.AppUser).AsQueryable();

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
                                : stocks.OrderBy(s => s.CompanyName);
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

                var skipNumber = (query.PageNumber - 1) * query.PageSize;
                var result = await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
                
                logger.LogInformation("Retrieved {Count} stocks with pagination (Page: {Page}, Size: {Size})", 
                    result.Count, query.PageNumber, query.PageSize);
                
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving stocks with query: {@Query}", query);
                throw;
            }
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return await context.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
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
            try
            {
                var existingStock = await context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

                if (existingStock == null)
                {
                    logger.LogWarning("Attempted to update non-existent stock with id: {Id}", id);
                    return null;
                }

                existingStock.Symbol = stockDTO.Symbol;
                existingStock.CompanyName = stockDTO.CompanyName;
                existingStock.Industry = stockDTO.Industry;
                existingStock.Price = stockDTO.Price;
                existingStock.LastDiv = stockDTO.LastDiv;
                existingStock.MarketCap = stockDTO.MarketCap;

                await context.SaveChangesAsync();
                logger.LogInformation("Successfully updated stock with id: {Id}, symbol: {Symbol}", 
                    id, existingStock.Symbol);

                return existingStock;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating stock with id: {Id}", id);
                throw;
            }
        }


    }
}
