using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EBIMa.Models;
using System.Threading.Tasks;
using EBIMa.DTO;
using Azure.Core;

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

		#region Resident Requests

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

		#endregion

		#region Payments

		[HttpGet("Payments")]
		public async Task<ActionResult<IEnumerable<GetPaymentFormsDTO>>> GetPayments()
		{
			var payments = await _context.PaymentForms
				.Include(u => u.User)
				.Select(p => new GetPaymentFormsDTO
				{
					Id = p.Id,
					FullName = (p.User.Name + " " + p.User.SurName),
					ApartmentNumber = p.User.ApartmentNumber,
					Month = p.Month,
					PaymentDate = p.PaymentDate,
					Status = p.Status,
					ImagePath = p.ImagePath
				}).ToListAsync();

			return Ok(payments);
		}

		[HttpPut("ApprovePayment/{paymentId}")]
		public async Task<IActionResult> ApprovePayment(Guid paymentId)
		{
			var payment = await _context.PaymentForms
				.Include(p => p.User)
				.SingleOrDefaultAsync(p => p.Id == paymentId);

			if (payment is null)
			{
				return NotFound("Payment not found.");
			}

			payment.Status = "Approved";
			payment.User.CurrentPayment = 0; // Ödəniş sıfırlanır
			await _context.SaveChangesAsync();

			return Ok("Payment Approved.");
		}

		[HttpPut("PendingPayment/{paymentId}")]
		public async Task<IActionResult> PendingPayment(Guid paymentId)
		{
			var payment = await _context.PaymentForms
				.Include(p => p.User)
				.SingleOrDefaultAsync(p => p.Id == paymentId);

			if (payment is null)
			{
				return NotFound("Payment not found.");
			}

			payment.Status = "Pending";
			await _context.SaveChangesAsync();

			return Ok("Payment Pending.");
		}

		[HttpPut("DeniedPayment/{paymentId}")]
		public async Task<IActionResult> DeniedPayment(Guid paymentId)
		{
			var payment = await _context.PaymentForms
				.Include(p => p.User)
				.SingleOrDefaultAsync(p => p.Id == paymentId);

			if (payment is null)
			{
				return NotFound("Payment not found.");
			}

			payment.Status = "Denied";
			await _context.SaveChangesAsync();

			return Ok("Payment Denied.");
		}

		#endregion

		#region  ApplicationRequests

		[HttpGet("ApplicationRequests")]
		public async Task<ActionResult<IEnumerable<GetApplicationRequestsDTO>>> GetApplicationRequestsAsync()
		{
			var userRequests = await _context.ApplicationRequests
				.Include(u => u.User)
				.Select(u => new GetApplicationRequestsDTO
				{
					RequestId = u.Id,
					FullName = u.User.Name + " " + u.User.SurName,
					ApartmentNumber = u.User.ApartmentNumber,
					RequestType = u.RequestType,
					CreatedAt = u.CreatedAt,
					Status = u.Status
				}).ToListAsync();

			return Ok(userRequests);

		}

		[HttpGet("ApplicationRequests/{requestId}")]
		public async Task<ActionResult<GetApplicationRequestsByIdDTO>> GetApplicationRequestsByIdAsync(Guid requestId)
		{
			var userRequests = await _context.ApplicationRequests
				.Include(u => u.User)
				.SingleOrDefaultAsync(u => u.Id == requestId);

			if (userRequests is null)
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

		[HttpPut("ApproveApplicationRequest/{requestId}")]
		public async Task<IActionResult> ApproveApplicationRequest(Guid requestId)
		{
			var request = await _context.ApplicationRequests
				.Include(u => u.User)
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

		[HttpPut("DeniedApplicationRequest/{requestId}")]
		public async Task<IActionResult> DeniedApplicationRequest(Guid requestId)
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

		[HttpPut("PendingApplicationRequest/{requestId}")]
		public async Task<IActionResult> PendingApplicationRequest(Guid requestId)
		{
			var request = await _context.ApplicationRequests
				.Include(u => u.User)
				.SingleOrDefaultAsync(r => r.Id == requestId);

			if (request is null)
			{
				return NotFound("Request not found.");
			}

			request.Status = "Pending";
			await _context.SaveChangesAsync();

			string subject = "Müraciətlər";
			string body = $"Sizin müraciətinizə baxılır.Təşəkürlər!";

			_emailService.SendEmail(request.User.Email, subject, body);

			return Ok("Request Pending.");

		}

		#endregion

		#region Home

		[HttpPost("Notification")]
		public async Task<IActionResult> SubmitNotification(string message)
		{
			var users = await _context.Users.ToListAsync();

			string subject = "Bildiriş";
			string body = $"<h3>{message}</h3>";

			foreach (var user in users)
			{
				_emailService.SendEmail(user.Email, subject, body);
			}

			return Ok("Bütün sakinlərə bildiriş göndərildi.");
		}

		[HttpGet("LastPayments")]
		public async Task<ActionResult<IEnumerable<LastPaymentsDTO>>> LastPaymentsAsync()
		{
			var payments = await _context.PaymentForms
				.Include(p => p.User)
				.Select(p => new LastPaymentsDTO
				{
					ApartmentNumber = p.User.ApartmentNumber,
					PaymentDate = p.PaymentDate,
					Month = p.Month,
					CurrentPayment = p.User.SquareMeterSize * 0.05M,
					Status = p.Status,
					ImagePath = p.ImagePath
				}).OrderByDescending(p => p.PaymentDate)
				.ToListAsync();

			return payments;
		}


		#endregion

		#region Apartment

		[HttpGet("Apartments")]
		public async Task<ActionResult<IEnumerable<ApartmentsDTO>>> GetUserApartmentsAsync()
		{
			var users = await _context.Users
				.Select(u => new ApartmentsDTO
				{
					UserId = u.Id,
					ApartmentNumber = u.ApartmentNumber,
					FullName = u.Name + " " + u.SurName,
					PhoneNumber = u.OwnerPhoneNumber
				}).ToListAsync();

			return users;
		}

		[HttpGet("Apartments/{userId}")]
		public async Task<ActionResult<GetUserByIdDTO>> GetUserByIdAsync(Guid userId)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

			if (user is null)
			{
				return BadRequest("İstifadəçi mövcud deyil!");
			}

			var userDto = new GetUserByIdDTO
			{
				Name = user.Name,
				Surname = user.SurName,
				Email = user.Email,
				PhoneNumber = user.OwnerPhoneNumber,
				MTK = user.MTK,
				BlockNumber = user.BlockNumber,
				Floor = user.Floor,
				ApartmentNumber = user.ApartmentNumber,
				SquareMeters = user.SquareMeterSize,
				MonthlyPayment = user.SquareMeterSize * 0.05M // Calculation
			};

			return Ok(userDto);

		}

		#endregion



		/*[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(Guid id)
		{
			var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);

			_context.Users.Remove(user);
			await _context.SaveChangesAsync();
			return Ok("silindi");
		*/


	}
}
	

