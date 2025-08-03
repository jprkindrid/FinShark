using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portfolios")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IStockRepository stockRepo;
        private readonly IPortfolioRepository portfolioRepo;
        private readonly IFMPService fmpService;
        private readonly ILogger<PortfolioController> logger;

        public PortfolioController(
            UserManager<AppUser> userManager,
            IStockRepository stockRepo,
            IPortfolioRepository portfolioRepo,
            IFMPService fmpService,
            ILogger<PortfolioController> logger)
        {
            this.userManager = userManager;
            this.stockRepo = stockRepo;
            this.portfolioRepo = portfolioRepo;
            this.fmpService = fmpService;
            this.logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPorfolio()
        {
            try
            {
                var username = User.GetUsername();
                if (string.IsNullOrEmpty(username))
                {
                    logger.LogWarning("Unauthorized portfolio access attempt - no username found");
                    return Unauthorized();
                }

                var appUser = await userManager.FindByNameAsync(username);
                if (appUser == null)
                {
                    logger.LogWarning("Portfolio access attempt for non-existent user: {Username}", username);
                    return NotFound("User not registered in database");
                }
                
                var userPortfolio = await portfolioRepo.GetUserPortfolio(appUser);
                logger.LogInformation("Retrieved portfolio for user: {Username} with {Count} stocks", 
                    username, userPortfolio.Count);
                
                return Ok(userPortfolio);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving portfolio");
                return StatusCode(500, "An error occurred while retrieving your portfolio");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            try
            {
                var username = User.GetUsername();
                if (string.IsNullOrEmpty(username))
                {
                    logger.LogWarning("Unauthorized portfolio add attempt - no username found");
                    return Unauthorized();
                }

                var appUser = await userManager.FindByNameAsync(username);
                if (appUser == null)
                {
                    logger.LogWarning("Portfolio add attempt for non-existent user: {Username}", username);
                    return BadRequest("User does not exist");
                }

                var stock = await stockRepo.GetBySymbolAsync(symbol);
                if (stock == null)
                {
                    stock = await fmpService.FindStockBySymbolAsync(symbol);
                    if (stock == null)
                    {
                        logger.LogWarning("Attempted to add non-existent stock to portfolio: {Symbol}", symbol);
                        return BadRequest("Stock does not exist");
                    }
                    await stockRepo.CreateAsync(stock);
                }

                var userPortfolio = await portfolioRepo.GetUserPortfolio(appUser);

                if (userPortfolio.Any(x => x.Symbol.ToLower() == symbol.ToLower()))
                {
                    logger.LogWarning("User {Username} attempted to add duplicate stock: {Symbol}", username, symbol);
                    return BadRequest("Stock already in user portfolio");
                }

                var portfolioModel = new Portfolio
                {
                    StockId = stock.Id,
                    AppUserId = appUser.Id
                };

                var createdPortfolio = await portfolioRepo.CreateAsync(portfolioModel);
                if (createdPortfolio == null)
                {
                    logger.LogError("Failed to create portfolio entry for user {Username}, stock {Symbol}", username, symbol);
                    return StatusCode(500, "Could not create portfolio");
                }

                logger.LogInformation("Successfully added stock {Symbol} to portfolio for user {Username}", symbol, username);
                return Created();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while adding stock {Symbol} to portfolio", symbol);
                return StatusCode(500, "An error occurred while adding the stock to your portfolio");
            }
        }


        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio(string symbol)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(symbol))
                {
                    logger.LogWarning("Delete portfolio attempt with empty symbol");
                    return BadRequest("Symbol is required.");
                }

                var username = User.GetUsername();
                if (string.IsNullOrEmpty(username))
                {
                    logger.LogWarning("Unauthorized portfolio delete attempt - no username found");
                    return Unauthorized();
                }

                var appUser = await userManager.FindByNameAsync(username);
                if (appUser == null)
                {
                    logger.LogWarning("Portfolio delete attempt for non-existent user: {Username}", username);
                    return Unauthorized("User not found");
                }

                var deleted = await portfolioRepo.DeletePortfolio(appUser, symbol);
                if (deleted == null)
                {
                    logger.LogWarning("User {Username} attempted to delete non-existent stock from portfolio: {Symbol}", username, symbol);
                    return BadRequest("Stock is not in your portfolio.");
                }

                logger.LogInformation("Successfully removed stock {Symbol} from portfolio for user {Username}", symbol, username);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while removing stock {Symbol} from portfolio", symbol);
                return StatusCode(500, "An error occurred while removing the stock from your portfolio");
            }
        }
    }
}
