using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Trading.Analytics.Domain;
using Trading.Analytics.Server.Utils;
using Trading.Analytics.Shared.Models;

namespace Trading.Analytics.Server.Services
{
    public class TradingSnapshotService : IInvocable
    {
        private readonly TradingContext context;
        private readonly HttpClient httpClient;
        private readonly ILogger<TradingSnapshotService> logger;

        public TradingSnapshotService(HttpClient httpClient, ILogger<TradingSnapshotService> logger,
            TradingContext context)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this.context = context;
        }

        public async Task Invoke()
        {
            try
            {
                var response = await httpClient.GetAsync($"portfolio");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var portfolio = JsonConvert.DeserializeObject<Portfolio>(content);
                    foreach (var p in portfolio.Payload.Positions)
                    {
                        var pos = await context.Positions.FirstOrDefaultAsync(position =>
                            position.Ticker.Equals(p.Ticker));
                        if (pos != null)
                            continue;
                        var newPos = new Position
                        {
                            Ticker = p.Ticker,
                            Figi = p.Figi,
                            Name = p.Name,
                            Currency = p.ExpectedYield.Currency
                        };
                        await context.Positions.AddAsync(newPos);
                        await context.SaveChangesAsync();
                    }

                    var positionSnapshots = new List<PositionSnapshot>();
                    foreach (var p in portfolio.Payload.Positions)
                    {
                        positionSnapshots.Add(new PositionSnapshot
                        {
                            Position = await context.Positions.FirstAsync(position => position.Ticker.Equals(p.Ticker)),
                            Count = Convert.ToInt32(p.Balance),
                            Value = p.AveragePositionPrice.Value + (p.ExpectedYield.Value / p.Balance)
                        });
                    }

                    var rubPerUsd = await RubPerUsd();
                    if (rubPerUsd > 0)
                    {
                        var portfolioSnapshot = new PortfolioSnapshot
                        {
                            DateTime = new DateTimeWithZone(DateTime.Now, TimeZoneInfo.Local).LocalTime,
                            Positions = positionSnapshots,
                            PriceRub = positionSnapshots.Where(snapshot => snapshot.Position.Currency.Equals("RUB"))
                                .Select(snapshot => snapshot.Price).Sum(),
                            PriceUsd = positionSnapshots.Where(snapshot => snapshot.Position.Currency.Equals("USD"))
                                .Select(snapshot => snapshot.Price).Sum(),
                            RubPerUsd = rubPerUsd
                        };
                        portfolioSnapshot.TotalPriceRub =
                            portfolioSnapshot.PriceRub + portfolioSnapshot.PriceUsd * portfolioSnapshot.RubPerUsd;
                        await context.PortfolioSnapshots.AddAsync(portfolioSnapshot);
                        await context.SaveChangesAsync();
                    }
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Exception("You have no access to that resource.");
                    throw new Exception("Something went wrong...");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }

        private async Task<double> RubPerUsd()
        {
            double rubPerUsd = 0;
            var now = DateTime.Now;
            var cd1Hour = now.Hour - 1;
            if (cd1Hour < 0)
                cd1Hour = 23;
            var figiParam = HttpUtility.UrlEncode("BBG0013HGFT4");
            var from = new DateTimeWithZone(new DateTime(now.Year, now.Month, now.Day, cd1Hour, now.Minute, now.Second),
                TimeZoneInfo.Local);
            var to = new DateTimeWithZone(now, TimeZoneInfo.Local);
            var fromParam = HttpUtility.UrlEncode(@from.LocalTime.ToString("O"));
            var toParam = HttpUtility.UrlEncode(to.LocalTime.ToString("O"));
            var query = $"?figi={figiParam}&from={fromParam}&to={toParam}&interval=hour";
            var response = await httpClient.GetAsync($"market/candles{query}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var stringAsync = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var rubPerUsdEnvelope = JsonConvert.DeserializeObject<RubPerUsd>(stringAsync);
                if (rubPerUsdEnvelope.Payload.Candles.Any())
                {
                    rubPerUsd = rubPerUsdEnvelope.Payload.Candles.First().C;
                }
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("You have no access to that resource.");
                throw new Exception("Something went wrong...");
            }

            return rubPerUsd;
        }
    }
}