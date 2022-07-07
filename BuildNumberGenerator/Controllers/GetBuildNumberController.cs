using Microsoft.AspNetCore.Mvc;

namespace BuildNumberGenerator.Controllers
{
   	[ApiController]
	[Route("api/getBuildNumber/")]
	public class GetBuildNumberController : ControllerBase
	{ 
        private ILogger<GetBuildNumberController> _logger;

        public GetBuildNumberController(ILogger<GetBuildNumberController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new OkResult();
        }
    }
}