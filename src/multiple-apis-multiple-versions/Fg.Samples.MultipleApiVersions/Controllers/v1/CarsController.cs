using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Fg.Samples.MultipleApiVersions.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "cars")]
    public class CarsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Cars v1");
        }
    }
}
