using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Organization.WebAPI.Controllers
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
			return Ok("Organization Web Api is running.");
		}
	}
}
