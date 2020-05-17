using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Trading.Analytics.Server.Utils;
using Trading.Analytics.Shared.Models;

namespace Trading.Analytics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController : BaseController<Operations>
    {
        public OperationsController(HttpClient httpClient, string endpoint = "operations") : base(httpClient, endpoint)
        {
        }

        [HttpGet]
        public async Task<IEnumerable<Operations.Operation>> Get()
        {
            var from = new DateTimeWithZone(new DateTime(2016, 1, 1, 0, 0, 0), TimeZoneInfo.Local);
            var to = new DateTimeWithZone(DateTime.Now, TimeZoneInfo.Local);
            var fromParam = HttpUtility.UrlEncode(from.LocalTime.ToString("O"));
            var toParam = HttpUtility.UrlEncode(to.LocalTime.ToString("O"));
            var query = $"from={fromParam}&to={toParam}";
            var result = await Get(query);
            return result.Payload.Operations;
        }
    }
}