using BuildNumberGenerator.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetBuildNumber()
        {
            Identity? id = AuthenticationHelper.GetAuthenticatedIdentity(this.HttpContext);

            if (id is null) // [UseAuthorization] should set this
            {
                throw new ArgumentNullException("id");
            }

            return new OkObjectResult(_generator.GetNextBuildNumber(id.Id, ""));
        }

        [HttpGet]
        [Route("ping/")]
        public IActionResult Ping()
        {
            return new OkObjectResult("Ping");
        }
    }
}