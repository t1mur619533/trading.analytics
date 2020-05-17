using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Trading.Analytics.Server.Controllers
{
    public abstract class BaseController<TPayload> : ControllerBase
    {
        protected readonly string Endpoint;
        protected readonly HttpClient HttpClient;

        protected BaseController(HttpClient httpClient, string endpoint = null)
        {
            Endpoint = endpoint;
            this.HttpClient = httpClient;
        }

        protected async Task<TPayload> Get(string query)
        {
            var response = await HttpClient.GetAsync($"{Endpoint}?{query}");
            return await HandleResponseAsync<TPayload>(response).ConfigureAwait(false);
        }

        private async Task<TOut> HandleResponseAsync<TOut>(HttpResponseMessage response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<TOut>(content);
                case HttpStatusCode.Unauthorized:
                    throw new Exception("You have no access to that resource.");
                default:
                    throw new Exception("Something went wrong...");
            }
        }
    }
}