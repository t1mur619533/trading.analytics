using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trading.Analytics.Domain;
using Trading.Analytics.Shared;

namespace Trading.Analytics.Monitoring.Controllers
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
        public async Task<IEnumerable<PortfolioStep>> Get([FromQuery] int page = 0, [FromQuery] int count = 1000)
        {
            var result = await context.PortfolioSnapshots
                .Select(snapshot => new PortfolioStep
                    {Date = snapshot.DateTime, Balance = Math.Round(snapshot.TotalPriceRub)})
                .Skip(page * count)
                .Take(count)
                .ToListAsync();
            return result;
        }
    }
}