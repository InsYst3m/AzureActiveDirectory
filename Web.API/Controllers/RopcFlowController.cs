using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Web.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    [RequiredScope("Api.Access")]
    public class RopcFlowController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "ROPC flow controller result.";
        }
    }
}
