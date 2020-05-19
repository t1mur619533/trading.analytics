using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trading.Analytics.Domain;
using Trading.Analytics.Shared;

namespace Trading.Analytics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioStepsController : ControllerBase
    {
        private readonly TradingContext context;

        public PortfolioStepsController(TradingContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<PortfolioStep>> Get()
        {
            return await context.PortfolioSnapshots
                .Select(snapshot => new PortfolioStep {Date = snapshot.DateTime, Balance = Math.Round(snapshot.TotalPriceRub)})
                .ToListAsync();
        }
    }
}