using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Trading.Analytics.Shared.Models;
using Trading.Analytics.Utils;

namespace Trading.Analytics.Services
{
    // ReSharper disable once UnusedMember.Global
    public class ExchangeRateService_BBG0013HGFT4 : IExchangeRateService
    {
        //FIGI для пары RUB - USD
        private string FigiParam => HttpUtility.UrlEncode("BBG0013HGFT4");
        private readonly ILogger<ExchangeRateService_BBG0013HGFT4> logger;
        private readonly ITinkoffApiClient tinkoffApiClient;

        public ExchangeRateService_BBG0013HGFT4(ITinkoffApiClient tinkoffApiClient,
            ILogger<ExchangeRateService_BBG0013HGFT4> logger)
        {
            this.tinkoffApiClient = tinkoffApiClient;
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task<Candle> GetCandle(DateTime dateTime)
        {
            return await GetAveragePrice(dateTime);
        }

        /// <inheritdoc />
        public async Task<double> GetDailyAveragePrice(DateTime dateTime)
        {
            var candle = await GetAveragePrice(dateTime);
            return (candle.O + candle.C) / 2;
        }


        private async Task<Candle> GetAveragePrice(DateTime dateTime)
        {
            var dateInterval = DateInterval(dateTime);
            var fromParam = HttpUtility.UrlEncode(dateInterval.Item1.ToString("O"));
            var toParam = HttpUtility.UrlEncode(dateInterval.Item2.ToString("O"));
            var query = $"?figi={FigiParam}&from={fromParam}&to={toParam}&interval=day";
            var response = await tinkoffApiClient.GetAsync($"market/candles{query}");
            var stringAsync = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var rubPerUsd = JsonConvert.DeserializeObject<Candles>(stringAsync);

                //Если в указанный промежуток биржа работала - мы получим свечу
                if (rubPerUsd.Payload.Candles != null && rubPerUsd.Payload.Candles.Count > 0)
                {
                    return rubPerUsd.Payload.Candles.Last();
                }

                //В случае, если биржа была закрыта, получаем свечу за прошлый день
                return await GetAveragePrice(dateTime.AddDays(-1));
            }

            logger.LogError($"{response.StatusCode} : {response.Content}");
            if (!string.IsNullOrEmpty(stringAsync))
            {
                var error = JsonConvert.DeserializeObject<OpenApiError>(stringAsync);
                logger.LogError($"{error.Status} : {error.Payload.Code} : {error.Payload.Message}");
            }

            throw new Exception($"{response.StatusCode} : {response.ReasonPhrase}");

            (DateTime, DateTime) DateInterval(DateTime _)
            {
                var from = new TZDateTime(_.AddDays(-1), TimeZoneInfo.Local);
                var to = new TZDateTime(_, TimeZoneInfo.Local);
                return (from.LocalTime, to.LocalTime);
            }
        }
    }
}