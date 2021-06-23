using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "DaemonAppRole")]
    public class ClientCredentialsFlowController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        public string[] Get()
        {
            return Summaries;
        }
    }
}
