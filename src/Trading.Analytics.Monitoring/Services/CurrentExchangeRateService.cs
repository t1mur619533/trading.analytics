using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Trading.Analytics.Monitoring.Utils;
using Trading.Analytics.Shared.Models;

namespace Trading.Analytics.Monitoring.Services
{
    public class CurrentRubPerUsdExchangeRateService
    {
        private readonly HttpClient httpClient;
        private double rubPerUsd;

        public CurrentRubPerUsdExchangeRateService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<double> LastHourClosingPrice()
        {
            var now = DateTime.Now;

            switch (now.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    now = new DateTime(now.Year, now.Month, now.Day - 1, 23, 45, 0);
                    break;
                case DayOfWeek.Sunday:
                    now = new DateTime(now.Year, now.Month, now.Day - 2, 23, 45, 0);
                    break;
                default:
                    now = DateTime.Now;
                    break;
            }

            if (now.Hour < 10)
            {
                now = now.DayOfWeek == DayOfWeek.Monday
                    ? new DateTime(now.Year, now.Month, now.Day - 3, 23, 45, 0)
                    : new DateTime(now.Year, now.Month, now.Day - 1, 23, 45, 0);
            }

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
                var rubPerUsdEnvelope = JsonConvert.DeserializeObject<Candles>(stringAsync);
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