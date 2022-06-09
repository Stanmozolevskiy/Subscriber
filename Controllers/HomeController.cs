using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sbuscriber.Controllers
{
	[Route("[controller]"), ApiController, AllowAnonymous]
	public class HomeController : ControllerBase
	{


		[HttpGet("{input}")]
		public IActionResult Eco(string input) => Ok(input);



	}
}