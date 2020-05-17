using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;
using Trading.Analytics.Server.Utils;
using Trading.Analytics.Shared;

namespace Trading.Analytics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        [HttpGet]
        public async Task<List<Asset>> Get()
        {
            try
            {
                var token = "mytoken";
                var connection = ConnectionFactory.GetConnection(token);
                var context = connection.Context;
                var portfolio = await context.PortfolioAsync();
                var allFigi = portfolio.Positions.Select(position => position.Figi);
                var portfolioCurrencies = await context.PortfolioCurrenciesAsync();
                List<Operation> operations = new List<Operation>();

                var from = new DateTimeWithZone(new DateTime(2019, 1, 1, 0, 0, 0), TimeZoneInfo.Local);
                var to = new DateTimeWithZone(DateTime.Now, TimeZoneInfo.Local);
                foreach (var figi in allFigi)
                {
                    operations.AddRange(await context.OperationsAsync(from.LocalTime, to.LocalTime, figi));
                }

                var fromParam = HttpUtility.UrlEncode(from.LocalTime.ToString("O"));
                var toParam = HttpUtility.UrlEncode(to.LocalTime.ToString("O"));

                var path = $"operations?from={fromParam}&to={toParam}";
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var uri = new Uri(new Uri("https://api-invest.tinkoff.ru/openapi/"), path);
                var response = await httpClient.GetAsync(uri).ConfigureAwait(false);
                string content;
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        break;
                }

                var stocks = new List<Asset>(portfolio.Positions.Count);
                stocks.AddRange(portfolio.Positions.Select(position =>
                    new Asset
                    {
                        Ticker = position.Ticker,
                        Balance = Convert.ToDouble(position.AveragePositionPrice.Value * position.Balance + position.ExpectedYield.Value),
                        Profit = Convert.ToDouble(position.ExpectedYield.Value),
                        Сurrency = position.ExpectedYield.Currency.ToString()
                    }));
                return stocks; //new StocksEnvelope { Assets = stocks };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}