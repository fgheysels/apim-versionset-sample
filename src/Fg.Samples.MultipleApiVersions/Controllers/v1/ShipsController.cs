using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Fg.Samples.MultipleApiVersions.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "ships")]
    public class ShipsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Ships v1");
        }
    }
}
