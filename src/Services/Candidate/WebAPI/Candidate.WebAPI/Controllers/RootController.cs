using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Candidate.WebAPI.Controllers
{
	[Route("")]
	[ApiController]
	public class RootController : Controller
	{
		[Route("")]
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Index()
		{
			return Ok("Candidate Web Api is running.");
		}
	}
}
