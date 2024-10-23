using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using EBIMa.Data;
using EBIMa.Models;

namespace EBIMa.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ApplicationRequestsController : ControllerBase
	{
		private readonly DataContext _context;

		public ApplicationRequestsController(DataContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<IActionResult> PostApplicationRequest([FromBody] ApplicationRequest request)
		{
			if (request == null || string.IsNullOrEmpty(request.RequestType) || string.IsNullOrEmpty(request.Message))
			{
				return BadRequest("Request type and message are required.");
			}

			_context.ApplicationRequests.Add(request);
			await _context.SaveChangesAsync();

			return Ok(new { Message = "Request submitted successfully!" });
		}

		[HttpGet]
		public async Task<IActionResult> GetApplicationRequests()
		{
			var requests = await _context.ApplicationRequests.ToListAsync();
			return Ok(requests);
		}
	}
}
