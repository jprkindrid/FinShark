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
            var username = User.GetUsername();
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var appUser = await userManager.FindByNameAsync(username);

            if (appUser == null)
            {
                return BadRequest("User does not exist");
            }

            var stock = await stockRepo.GetBySymbolAsync(symbol);
            if (stock == null)
            {
                stock = await fmpService.FindStockBySymbolAsync(symbol);
                if (stock == null)
                {
                    return BadRequest("Stock does not exist");
                }
                await stockRepo.CreateAsync(stock);
            }

            var userPortfolio = await portfolioRepo.GetUserPortfolio(appUser);

            if (userPortfolio.Any(x => x.Symbol.ToLower() == symbol.ToLower()))
                return BadRequest("Stock already in user portfolio");

            var portfolioModel = new Portfolio
            {
                StockId = stock.Id,
                AppUserId = appUser.Id
            };

            await portfolioRepo.CreateAsync(portfolioModel);

            if (portfolioModel == null)
                return StatusCode(500, "Could not create portfolio");

            return Created();

        }


        [HttpDelete]
        [Authorize]

        public async Task <IActionResult> DeletePortfolio(string symbol)
        {

            if (string.IsNullOrWhiteSpace(symbol))
                return BadRequest("Symbol is required.");

            var username = User.GetUsername();
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var appUser = await userManager.FindByNameAsync(username);
            if (appUser is null)
                return Unauthorized(); var stock = await stockRepo.GetBySymbolAsync(symbol);

            var deleted = await portfolioRepo.DeletePortfolio(appUser, symbol);
            if (deleted is null)
                return BadRequest("Stock is not in your portfolio.");

            return NoContent();

        }
    }
}
