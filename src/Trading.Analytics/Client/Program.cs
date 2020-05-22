using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Blazor;
using Trading.Analytics.Shared;

namespace Trading.Analytics.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddBaseAddressHttpClient();
            builder.Services.AddSyncfusionBlazor();
            var configuration = await GetConfiguration(builder.Services);
            builder.Services.AddSingleton(s => configuration);
            await builder.Build().RunAsync();

            async Task<Endpoints> GetConfiguration(IServiceCollection services)
            {
                await using var provider = services.BuildServiceProvider();
                var baseUri = provider.GetRequiredService<NavigationManager>().BaseUri;
                var url = $"{(baseUri.EndsWith('/') ? baseUri : baseUri + "/")}api/Endpoints";
                using var client = new HttpClient();
                return await client.GetJsonAsync<Endpoints>(url);
            }
        }
    }
}