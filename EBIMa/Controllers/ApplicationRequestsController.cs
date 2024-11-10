using EBIMa.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EBIMa.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ApplicationRequestsController : ControllerBase
	{
		private readonly DataContext _context;

		public ApplicationRequestsController(DataContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<IActionResult> PostApplicationRequest([FromBody] PostApplicationRequestsDTO request)
		{
			if (request == null || string.IsNullOrEmpty(request.RequestType) || string.IsNullOrEmpty(request.Message))
			{
				return BadRequest("Request type and message are required.");
			}

			
			var newRequests = new ApplicationRequest
			{
				UserId = request.UserId,
				RequestType = request.RequestType,
				Message = request.Message,
				Status = "Pending",
				CreatedAt = DateTime.UtcNow
			};

			await _context.ApplicationRequests.AddAsync(newRequests);

			await _context.SaveChangesAsync();

			return Ok(new { Message = "Request submitted successfully!" });
		}

		[HttpGet]
		public async Task<IActionResult> GetApplicationRequests(Guid userId)
		{
			var requests = await _context.ApplicationRequests
				.Where(r => r.UserId == userId)
				.Select(r => new UserApplicationRequestsDTO
				{
					CreatedAt = r.CreatedAt,
					RequestType = r.RequestType,
					Status = r.Status
				})
				.OrderByDescending(r => r.CreatedAt)
				.ToListAsync();


			return Ok(requests);
		}
	}
}
