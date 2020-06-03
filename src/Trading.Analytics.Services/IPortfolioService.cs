using System.Threading.Tasks;

namespace Trading.Analytics.Services
{
    public interface IPortfolioService
    {
        Task<Shared.Models.Portfolio> GetPortfolioAsync();
    }

    public class PortfolioService : IPortfolioService
    {

        public Task<Shared.Models.Portfolio> GetPortfolioAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
