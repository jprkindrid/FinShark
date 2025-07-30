using api.Data;
using api.DTOs.Stock;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IStockRepository stockRepo;

        public StockController(ApplicationDbContext context, IStockRepository stockRepo)
        {
            this.context = context;
            this.stockRepo = stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStocks()
        {
            var stocks = await stockRepo.GetAllAsync();

            var stockDTO = stocks.Select(s => s.ToStockDTO());

            return Ok(stockDTO);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStockById([FromRoute] int id)
        {
            var stock = await context.Stocks.FindAsync(id);

            if (stock == null)
                return NotFound();

            return Ok(stock.ToStockDTO());

        }

        [HttpPost]
        public async Task<IActionResult> CreateStock([FromBody] CreateStockRequestDTO stockDto)
        {
            var stockModel = stockDto.ToStockFromCreateDTO();
            await context.Stocks.AddAsync(stockModel);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetStockById), new { id = stockModel.Id }, stockModel.ToStockDTO());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateStock([FromRoute] int id, [FromBody] UpdateStockRequestDTO updateDto)
        {
            var stockModel = await context.Stocks.FindAsync(id);

            if (stockModel == null)
            {
                return NotFound();
            }

            stockModel.Symbol = updateDto.Symbol;
            stockModel.CompanyName = updateDto.CompanyName;
            stockModel.Industry = updateDto.Industry;
            stockModel.Price = updateDto.Price;
            stockModel.LastDiv = updateDto.LastDiv;
            stockModel.MarketCap = updateDto.MarketCap;

            await context.SaveChangesAsync();

            return Ok(stockModel.ToStockDTO());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteStock([FromRoute] int id)
        {
            var stockModel = await context.Stocks.FindAsync(id);
            if (stockModel == null)
            {
                return NotFound();
            }


            context.Stocks.Remove(stockModel);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
