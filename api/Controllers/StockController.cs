using api.DTOs.Stock;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/stocks")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository stockRepo;
        private readonly ILogger<StockController> logger;

        public StockController(IStockRepository stockRepo, ILogger<StockController> logger)
        {
            this.stockRepo = stockRepo;
            this.logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllStocks([FromQuery] QueryObject query)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model state for GetAllStocks request");
                return BadRequest(ModelState);
            }

            try
            {
                var stocks = await stockRepo.GetAllAsync(query);
                var stockDTO = stocks.Select(s => s.ToStockDTO()).ToList();
                
                logger.LogInformation("Retrieved {Count} stocks with query: {Query}", stockDTO.Count, query.ToString());
                return Ok(stockDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving stocks");
                return StatusCode(500, "An error occurred while retrieving stocks");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStockById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model state for GetStockById request with id: {Id}", id);
                return BadRequest(ModelState);
            }

            try
            {
                var stock = await stockRepo.GetByIdAsync(id);

                if (stock == null)
                {
                    logger.LogWarning("Stock not found with id: {Id}", id);
                    return NotFound();
                }

                logger.LogInformation("Retrieved stock with id: {Id}", id);
                return Ok(stock.ToStockDTO());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving stock with id: {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the stock");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateStock([FromBody] CreateStockRequestDTO stockDto)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model state for CreateStock request");
                return BadRequest(ModelState);
            }

            try
            {
                var stockModel = stockDto.ToStockFromCreateDTO();
                await stockRepo.CreateAsync(stockModel);
                
                logger.LogInformation("Successfully created stock with id: {Id}, symbol: {Symbol}", 
                    stockModel.Id, stockModel.Symbol);
                
                return CreatedAtAction(nameof(GetStockById), new { id = stockModel.Id }, stockModel.ToStockDTO());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating stock with symbol: {Symbol}", stockDto.Symbol);
                return StatusCode(500, "An error occurred while creating the stock");
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateStock([FromRoute] int id, [FromBody] UpdateStockRequestDTO updateDto)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model state for UpdateStock request with id: {Id}", id);
                return BadRequest(ModelState);
            }

            try
            {
                var stockModel = await stockRepo.UpdateAsync(id, updateDto);

                if (stockModel == null)
                {
                    logger.LogWarning("Stock not found for update with id: {Id}", id);
                    return NotFound();
                }

                logger.LogInformation("Successfully updated stock with id: {Id}, symbol: {Symbol}", 
                    stockModel.Id, stockModel.Symbol);
                return Ok(stockModel.ToStockDTO());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating stock with id: {Id}", id);
                return StatusCode(500, "An error occurred while updating the stock");
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteStock([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model state for DeleteStock request with id: {Id}", id);
                return BadRequest(ModelState);
            }

            try
            {
                var stockModel = await stockRepo.DeleteAsync(id);
                if (stockModel == null)
                {
                    logger.LogWarning("Stock not found for deletion with id: {Id}", id);
                    return NotFound();
                }

                logger.LogInformation("Successfully deleted stock with id: {Id}, symbol: {Symbol}", 
                    stockModel.Id, stockModel.Symbol);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting stock with id: {Id}", id);
                return StatusCode(500, "An error occurred while deleting the stock");
            }
        }
    }
}
