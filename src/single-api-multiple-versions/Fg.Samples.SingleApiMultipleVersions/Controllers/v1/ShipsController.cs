using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Fg.Samples.SingleApiMultipleVersions.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ShipsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Vessels v1");
        }
    }
}
