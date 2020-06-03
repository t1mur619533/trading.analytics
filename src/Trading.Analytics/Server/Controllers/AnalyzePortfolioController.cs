using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Trading.Analytics.Services;
using Trading.Analytics.Shared;
using Trading.Analytics.Shared.Models;
using Trading.Analytics.Utils;

namespace Trading.Analytics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyzePortfolioController : ControllerBase
    {
        private readonly ITinkoffApiClient tinkoffApiClient;
        private readonly ExchangeRateService_BBG0013HGFT4 rubPerUsdExchangeRateService;

        public AnalyzePortfolioController(ITinkoffApiClient tinkoffApiClient,
            ExchangeRateService_BBG0013HGFT4 rubPerUsdExchangeRateService)
        {
            this.tinkoffApiClient = tinkoffApiClient;
            this.rubPerUsdExchangeRateService = rubPerUsdExchangeRateService;
        }

        [HttpGet]
        public async Task<double> Get()
        {
            var from = new TZDateTime(new DateTime(2016, 1, 1, 0, 0, 0), TimeZoneInfo.Local);
            var to = new TZDateTime(DateTime.Now, TimeZoneInfo.Local);
            var fromParam = HttpUtility.UrlEncode(from.LocalTime.ToString("O"));
            var toParam = HttpUtility.UrlEncode(to.LocalTime.ToString("O"));
            var query = $"from={fromParam}&to={toParam}";
            var operationsResponse = await tinkoffApiClient.GetAsync($"operations?{query}");
            var operations = await HandleResponseAsync<Operations>(operationsResponse);

            var portfolioResponse = await tinkoffApiClient.GetAsync($"portfolio");
            var portfolio = await HandleResponseAsync<Portfolio>(portfolioResponse);
            return await Analyze(operations, portfolio);
        }

        private async Task<TOut> HandleResponseAsync<TOut>(HttpResponseMessage response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TOut>(content);
                case HttpStatusCode.Unauthorized:
                    throw new Exception("You have no access to that resource.");
                default:
                    throw new Exception("Something went wrong...");
            }
        }

        private async Task<double> Analyze(Operations operations, Portfolio portfolio)
        {
            var assets = new List<Asset>();
            var lastHourClosingPrice = await rubPerUsdExchangeRateService.GetDailyAveragePrice(DateTime.Now);
            assets.AddRange(portfolio.Payload.Positions.Select(position =>
                new Asset
                {
                    Ticker = position.Ticker,
                    Balance = Convert.ToDouble(position.AveragePositionPrice.Value * position.Balance +
                                               position.ExpectedYield.Value),
                    Profit = Convert.ToDouble(position.ExpectedYield.Value),
                    Сurrency = position.ExpectedYield.Currency,
                    RubPerUsd = lastHourClosingPrice,
                }));
            assets.ForEach(asset =>
            {
                asset.UsdBalance = asset.Сurrency.Equals("USD") ? asset.Balance : asset.Balance / asset.RubPerUsd;
                asset.UsdProfit = asset.Сurrency.Equals("USD") ? asset.Profit : asset.Profit / asset.RubPerUsd;
                asset.RubBalance = asset.Сurrency.Equals("RUB") ? asset.Balance : asset.Balance * asset.RubPerUsd;
                asset.RubProfit = asset.Сurrency.Equals("RUB") ? asset.Profit : asset.Profit * asset.RubPerUsd;
            });
            var totalPortfolioPrice = assets.Sum(asset => asset.RubBalance);

            double res = 0;

            foreach (var operation in operations.Payload.Operations)
            {
                if ((operation.OperationType.Equals("BuyCard") || operation.OperationType.Equals("PayIn")) && operation.Status.Equals("Done"))
                {
                    if (operation.Currency.Equals("USD"))
                    {
                        var rubPerUsd = await rubPerUsdExchangeRateService.GetDailyAveragePrice(operation.Date);
                        if (operation.Trades != null)
                        {
                            var payment = operation.Trades.Sum(operationTrade =>
                                operationTrade.Price * operationTrade.Quantity);
                            res += payment * rubPerUsd;
                        }
                        else
                        {
                            res += operation.Payment * rubPerUsd;
                        }
                    }
                    else
                    if (operation.Currency.Equals("RUB"))
                    {
                        if (operation.Trades != null)
                        {
                            var payment = operation.Trades.Sum(operationTrade =>
                                operationTrade.Price * operationTrade.Quantity);
                            res += payment;
                        }
                        else
                        {
                            res += operation.Payment;
                        }
                    }
                }
                else
                {
                    if (operation.OperationType.Equals("PayOut") || operation.OperationType.Equals("BrokerCommission") && operation.Status.Equals("Done"))
                    {
                        if (operation.Currency.Equals("USD"))
                        {
                            var rubPerUsd = await rubPerUsdExchangeRateService.GetDailyAveragePrice(operation.Date);
                            res += operation.Payment * rubPerUsd;
                        }
                        else
                        if (operation.Currency.Equals("RUB"))
                        {
                            res += operation.Payment;
                        }
                    }
                }
            }

            return totalPortfolioPrice - res;
        }
    }
}