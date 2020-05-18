using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Trading.Analytics.Server.Services;
using Trading.Analytics.Shared;
using Trading.Analytics.Shared.Models;

namespace Trading.Analytics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionsController : BaseController<Portfolio>
    {
        private readonly CurrentRubPerUsdExchangeRateService exchangeRateService;

        public PositionsController(HttpClient httpClient, CurrentRubPerUsdExchangeRateService exchangeRateService, string endpoint = "portfolio") :
            base(httpClient, endpoint)
        {
            this.exchangeRateService = exchangeRateService;
        }

        [HttpGet]
        public async Task<IEnumerable<Asset>> Get()
        {
            var result = await Get("");
            var assets = new List<Asset>();
            var rubPerUsd = await exchangeRateService.LastHourClosingPrice();
            assets.AddRange(result.Payload.Positions.Select(position =>
                new Asset
                {
                    Ticker = position.Ticker,
                    Balance = Convert.ToDouble(position.AveragePositionPrice.Value * position.Balance +
                                               position.ExpectedYield.Value),
                    Profit = Convert.ToDouble(position.ExpectedYield.Value),
                    Сurrency = position.ExpectedYield.Currency,
                    RubPerUsd = rubPerUsd,
                }));
            assets.ForEach(asset =>
            {
                asset.UsdBalance = asset.Сurrency.Equals("USD") ? asset.Balance : asset.Balance / asset.RubPerUsd;
                asset.UsdProfit = asset.Сurrency.Equals("USD") ? asset.Profit : asset.Profit / asset.RubPerUsd;
                asset.RubBalance = asset.Сurrency.Equals("RUB") ? asset.Balance : asset.Balance * asset.RubPerUsd;
                asset.RubProfit = asset.Сurrency.Equals("RUB") ? asset.Profit : asset.Profit * asset.RubPerUsd;
            });
            return assets.OrderBy(asset => asset.Profit);
        }
    }
}