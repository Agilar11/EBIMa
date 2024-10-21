using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EBIMa.Models;
using System.Threading.Tasks;

namespace EBIMa.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SuperintendentProfileController : ControllerBase
	{
		private readonly DataContext _context;

		public SuperintendentProfileController(DataContext context)
		{
			_context = context;
		}

		// Get all requests for a specific superintendent
		[HttpGet("GetRequests")]
		public async Task<IActionResult> GetRequests()
		{
			// Assume the superintendent is identified by their logged-in user ID.
			// Fetch all resident requests that have not yet been approved or denied.
			var requests = await _context.ResidentRequests
				.Include(r => r.Resident)
				.Where(r => r.IsApproved == null) // Fetch pending requests
				.ToListAsync();

			return Ok(requests);
		}

		// Approve a resident request
		[HttpPost("ApproveRequest/{requestId}")]
		public async Task<IActionResult> ApproveRequest(int requestId)
		{
			var request = await _context.ResidentRequests.FindAsync(requestId);
			if (request == null)
			{
				return NotFound("Request not found.");
			}

			request.IsApproved = true; // Approve the request
			await _context.SaveChangesAsync();

			return Ok("Request approved.");
		}

		// Deny a resident request
		[HttpPost("DenyRequest/{requestId}")]
		public async Task<IActionResult> DenyRequest(int requestId)
		{
			var request = await _context.ResidentRequests.FindAsync(requestId);
			if (request == null)
			{
				return NotFound("Request not found.");
			}

			request.IsApproved = false; // Deny the request
			await _context.SaveChangesAsync();

			return Ok("Request denied.");
		}



		[HttpGet("admin/forms")]
		public IActionResult GetAllForms()
		{
			var forms = _context.PaymentForms.ToList();
			return Ok(forms);
		}

	}
}
