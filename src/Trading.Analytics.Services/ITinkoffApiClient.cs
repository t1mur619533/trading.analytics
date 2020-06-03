using System.Net.Http;
using System.Threading.Tasks;
using Trading.Analytics.Shared.Models;

namespace Trading.Analytics.Services
{
    public interface ITinkoffApiClient
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);

        Task<OpenApiResponse<T>> GetAsync<T>(string requestUri);
    }
}