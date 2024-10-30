using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EBIMa.Models;
using System.Threading.Tasks;
using EBIMa.DTO;

namespace EBIMa.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class KamendantProfileController : ControllerBase
	{
		private readonly DataContext _context;
		private readonly IEmailService _emailService;

		public KamendantProfileController(DataContext context, IEmailService emailService)
		{
			_context = context;
			_emailService = emailService;
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


		// Payment

		[HttpPost("ApprovePayment/{userId}")]
		public async Task<IActionResult> ApprovePayment(int userId)
		{
			var user = await _context.Users
				.Include(u => u.PaymentForms)
				.SingleOrDefaultAsync(u => u.Id == userId);

			if (user == null)
			{
				return NotFound("İstifadəçi tapılmadı.");
			}

			user.CurrentPayment = 0; // Ödəniş sıfırlanır
			await _context.SaveChangesAsync();

			return Ok("Ödəniş sıfırlandı.");
		}

		// ApplicationRequest

		[HttpGet]
		public async Task<ActionResult<IEnumerable<GetApplicationRequestsDTO>>> GetApplicationRequestsAsync()
		{
			var userRequests = await _context.ApplicationRequests
				.Include(u => u.User)
				.Select(u => new GetApplicationRequestsDTO
				{
					ApartmentNumber = u.User.ApartmentNumber,
					RequestType = u.RequestType,
					CreatedAt = u.CreatedAt,
					Status = u.Status
				}).ToListAsync();

			return Ok(userRequests);

		}

		[HttpGet("{requestId}")]
		public async Task<ActionResult<GetApplicationRequestsByIdDTO>> GetApplicationRequestsByIdAsync(int requestId)
		{
			var userRequests = await _context.ApplicationRequests
				.Include(u => u.User)
				.SingleOrDefaultAsync (u => u.Id == requestId);

			if(userRequests is null)
			{
				return NotFound("Request not found");
			}

			var applicationRequest = new GetApplicationRequestsByIdDTO
			{
				ApartmentNumber = userRequests.User.ApartmentNumber,
				RequestType = userRequests.RequestType,
				CreatedAt = userRequests.CreatedAt,
				Status = userRequests.Status,
				Message = userRequests.Message
			};

			return Ok(applicationRequest);

		}

		[HttpPost("ApproveApplicationRequest/{requestId}")]
		public async Task<IActionResult> ApproveApplicationRequest(int requestId)
		{
			var request = await _context.ApplicationRequests
				.Include (u => u.User)
				.SingleOrDefaultAsync(r => r.Id == requestId);
			
			if (request is null)
			{
				return NotFound("Request not found.");
			}

			request.Status = "Approved"; 
			await _context.SaveChangesAsync();

			string subject = "Müraciətlər";
			string body = $"Sizin müraciətiniz təsdiq olundu.Təşəkürlər!";

			_emailService.SendEmail(request.User.Email, subject, body);

			return Ok("Request Approved.");
		}

		[HttpPost("DeniedApplicationRequest/{requestId}")]
		public async Task<IActionResult> DeniedApplicationRequest(int requestId)
		{
			var request = await _context.ApplicationRequests
				.Include(u => u.User)
				.SingleOrDefaultAsync(r => r.Id == requestId);

			if (request is null)
			{
				return NotFound("Request not found.");
			}

			request.Status = "Denied";
			await _context.SaveChangesAsync();

			string subject = "Müraciətlər";
			string body = $"Sizin müraciətiniz rədd edildi.Təşəkürlər!";

			_emailService.SendEmail(request.User.Email, subject, body);

			return Ok("Request Denied.");

		}


	}
}
