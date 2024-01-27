using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Fg.Samples.SingleApiMultipleVersions.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class CarsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Cars v2");
        }
    }
}
