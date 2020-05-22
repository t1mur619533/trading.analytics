using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Trading.Analytics.Shared;

namespace Trading.Analytics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EndpointsController : ControllerBase
    {
        private readonly IOptions<Endpoints> options;

        public EndpointsController(IOptions<Endpoints> options)
        {
            this.options = options;
        }

        [HttpGet]
        public Endpoints Get()
        {
            return options.Value;
        }
    }
}
