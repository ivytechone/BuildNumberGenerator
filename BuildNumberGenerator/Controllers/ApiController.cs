using Microsoft.AspNetCore.Mvc;
using IvyTech.Logging;

namespace BuildNumberGenerator.Controllers
{
   	[ApiController]
	public class ApiController : ControllerBase
	{ 
        private ILogger<ApiController> _logger;
        private IGenerator _generator;

        public ApiController(ILogger<ApiController> logger, IGenerator generator)
        {
            _logger = logger;
            _generator = generator;
        }

        [UseAuthentication]
        [UseAuthorization]
        [HttpGet]
        [Route("getBuildNumber/")]
        public IActionResult GetBuildNumber([FromQuery] string branch)
        {
            Identity? id = AuthenticationHelper.GetAuthenticatedIdentity(this.HttpContext);

            if (id is null || id.Id is null || id.TZ is null) // [UseAuthentication] should set this
            {
                throw new ArgumentNullException("id");
            }
            

            var result = _generator.GetNextBuildNumber(id.Id, branch, id.TZ);
            _logger.LogInformation("{result}", result);

            return new OkObjectResult(result);

        }

        [HttpGet]
        [Route("ping/")]
        public IActionResult Ping()
        {
            return new OkObjectResult("Ping");
        }
    }
}