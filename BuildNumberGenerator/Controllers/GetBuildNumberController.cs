using BuildNumberGenerator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BuildNumberGenerator.Controllers
{
   	[ApiController]
	[Route("api/getBuildNumber/")]
	public class GetBuildNumberController : ControllerBase
	{ 
        private ILogger<GetBuildNumberController> _logger;

        private IGenerator _generator;

        public GetBuildNumberController(ILogger<GetBuildNumberController> logger, IGenerator generator)
        {
            _logger = logger;
            _generator = generator;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return new OkObjectResult(_generator.GetNextBuildNumber("", ""));
        }
    }
}