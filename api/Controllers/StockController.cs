using api.Data;
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
    }
}
