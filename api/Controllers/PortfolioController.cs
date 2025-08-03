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

        public PortfolioController(
            UserManager<AppUser> userManager,
            IStockRepository stockRepo,
            IPortfolioRepository portfolioRepo)
        {
            this.userManager = userManager;
            this.stockRepo = stockRepo;
            this.portfolioRepo = portfolioRepo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPorfolio()
        {
            var username = User.GetUsername();
            var appUser = await userManager.FindByNameAsync(username);
            if (appUser == null)
                return NotFound("User not registered in database");
            var userPortfolio = await portfolioRepo.GetUserPortfolio(appUser);
            return Ok(userPortfolio);

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var username = User.GetUsername();
            var appUser = await userManager.FindByNameAsync(username);
            var stock = await stockRepo.GetBySymbolAsync(symbol);

            if (stock == null)
                return BadRequest("Stock not found");
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
