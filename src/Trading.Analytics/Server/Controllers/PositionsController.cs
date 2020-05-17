using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Trading.Analytics.Shared;
using Trading.Analytics.Shared.Models;

namespace Trading.Analytics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionsController : BaseController<Portfolio>
    {
        private readonly HttpClient httpClient;

        public PositionsController(HttpClient httpClient, IConfiguration configuration, string endpoint = "portfolio") :
            base(httpClient, endpoint)
        {
            this.httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IEnumerable<Asset>> Get()
        {
            var result = await Get("");
            var assets = new List<Asset>();
            assets.AddRange(result.Payload.Positions.Select(position =>
                new Asset
                {
                    Ticker = position.Ticker,
                    Balance = Convert.ToDouble(position.AveragePositionPrice.Value * position.Balance +
                                               position.ExpectedYield.Value),
                    Profit = Convert.ToDouble(position.ExpectedYield.Value),
                    Сurrency = position.ExpectedYield.Currency
                }));
            return assets;
        }
    }
}