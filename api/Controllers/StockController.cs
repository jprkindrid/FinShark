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
            var stock = await stockRepo.GetByIdAsync(id);

            if (stock == null)
                return NotFound();

            return Ok(stock.ToStockDTO());

        }

        [HttpPost]
        public async Task<IActionResult> CreateStock([FromBody] CreateStockRequestDTO stockDto)
        {
            var stockModel = stockDto.ToStockFromCreateDTO();
            await stockRepo.CreateAsync(stockModel);
            return CreatedAtAction(nameof(GetStockById), new { id = stockModel.Id }, stockModel.ToStockDTO());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateStock([FromRoute] int id, [FromBody] UpdateStockRequestDTO updateDto)
        {
            var stockModel = await stockRepo.UpdateAsync(id, updateDto);

            if (stockModel == null)
            {
                return NotFound();
            }

            return Ok(stockModel.ToStockDTO());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteStock([FromRoute] int id)
        {
            var stockModel = await stockRepo.DeleteAsync(id);
            if (stockModel == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
