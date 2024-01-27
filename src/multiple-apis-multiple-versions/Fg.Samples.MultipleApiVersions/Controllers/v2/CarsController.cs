using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Fg.Samples.MultipleApiVersions.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "cars")]
    public class CarsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Cars v2");
        }
    }
}
