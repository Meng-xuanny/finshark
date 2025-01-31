using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;

        private readonly IPortfolioRepository _portfolioRepo;

        public PortfolioController(
            UserManager<AppUser> userManager,
            IStockRepository repository,
            IPortfolioRepository portfolioRepo
        )
        {
            _userManager = userManager;
            _stockRepo = repository;
            _portfolioRepo = portfolioRepo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portfolioRepo.GetUserPortfolio(user);
            return Ok(userPortfolio);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var username = User.GetUsername();
            var user = await _userManager.FindByNameAsync(username);
            var stock=await _stockRepo.GetBySymbolAsync(symbol);

            if (stock==null) return BadRequest("Stock not found");

            var portfolio = await _portfolioRepo.GetUserPortfolio(user);
            if(portfolio.Any(p=>p.Symbol.ToLower() ==symbol.ToLower())) return BadRequest("Cannot add same stock to portfolio");

            var portfolioModel= new Portfolio
            {
                AppUserId=user.Id,
                StockId=stock.Id
            };

            await _portfolioRepo.CreateAsync(portfolioModel);
            if (portfolioModel == null)
            {
                return StatusCode(500, "Could not create");
            }
            else
            {
                return Created();
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio(string symbol)
        {
            var username= User.GetUsername();
            var user=await _userManager.FindByNameAsync(username);
            var userPortfolio=await _portfolioRepo.GetUserPortfolio(user);

            var filteredStock=userPortfolio.Where(p=>p.Symbol.ToLower()==symbol.ToLower());
            
            if (filteredStock.Count()==1)
            {
                await _portfolioRepo.DeletePortfolio(user, symbol);
            }
            else
            {
                return BadRequest("Stock not in your portfolio");
            }

            return Ok();
        }
    }
}
