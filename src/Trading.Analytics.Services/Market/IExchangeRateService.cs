using System;
using System.Threading.Tasks;
using Trading.Analytics.Shared.Models;

namespace Trading.Analytics.Services
{
    public interface IExchangeRateService
    {
        /// <summary>
        /// Получение исторических значений, дневная свеча
        /// </summary>
        /// <param name="dateTime"> Дата </param>
        /// <returns> Значения дневной свечи </returns>
        Task<Candle> GetCandle(DateTime dateTime);

        /// <summary>
        /// Получение исторических значений, реднее значение за день
        /// </summary>
        /// <param name="dateTime"> Дата </param>
        /// <returns> Среднее значение за день </returns>
        Task<double> GetDailyAveragePrice(DateTime dateTime);
    }
}