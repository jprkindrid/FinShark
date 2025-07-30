using api.Data;
using api.DTOs.Stock;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController :ControllerBase
    {
        private readonly ApplicationDbContext context;

        public StockController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetAllStocks()
        {
            var stocks = context.Stocks.ToList();

            return Ok(stocks);
        }

        [HttpGet("{id}")]
        public IActionResult GetStockByID([FromRoute] int id)
        {
            var stock = context.Stocks.Find(id);

            if ( stock == null)
                return NotFound();

            return Ok(stock);

        }

        [HttpPost]
        public IActionResult CreateStock([FromBody] CreateStockRequestDTO stockDto)
        {
            var stockModel = stockDto.ToStockFromCreateDTO();
            context.Stocks.Add(stockModel);
            context.SaveChanges();
            return CreatedAtAction(nameof(GetStockByID), new {id = stockModel.Id }, stockModel.ToStockDTO());
        }

        
    }
}
