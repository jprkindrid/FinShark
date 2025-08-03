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
    }
}
