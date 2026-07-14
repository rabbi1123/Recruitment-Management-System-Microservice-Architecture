using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Job.WebAPI.Controllers
{
	[Route("")]
	[ApiController]
	public class RootController : Controller
	{
		[Route("")]
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Index()
		{
			return Ok("Job Web Api is running.");
		}
	}
}
