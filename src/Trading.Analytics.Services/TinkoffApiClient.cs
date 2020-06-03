using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Trading.Analytics.Shared.Models;

namespace Trading.Analytics.Services
{
    public class TinkoffApiClient : ITinkoffApiClient
    {
        private readonly HttpClient httpClient;

        public TinkoffApiClient(IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            this.httpClient = httpClient;
            if (!httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
                return;
            var token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString()
                .Replace("Bearer ", "");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await httpClient.GetAsync(requestUri);
        }

        public async Task<OpenApiResponse<T>> GetAsync<T>(string requestUri)
        {
            var response = await GetAsync(requestUri);
            var stringAsync = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<OpenApiResponse<T>>(stringAsync);
            }

            throw new Exception($"{response.StatusCode} : {response.ReasonPhrase}");
        }
    }
}