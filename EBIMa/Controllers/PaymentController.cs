using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;
using EBIMa.DTO;
using static System.Net.Mime.MediaTypeNames;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace EBIMa.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PaymentController : ControllerBase
	{
		private readonly DataContext _context;
		private readonly IConfiguration _configuration;

		public PaymentController(DataContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}


		[HttpPost("submit")]
		public async Task<IActionResult> SubmitForm([FromForm] SubmitFormDTO form, IFormFile image)
		{
			if (ModelState.IsValid)
			{
				if (image != null && image.Length > 0)
				{
					// Get the upload path from configuration
					string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), _configuration.GetValue<string>("FileStorage:UploadPath"));

					// Ensure the directory exists
					if (!Directory.Exists(uploadPath))
					{
						Directory.CreateDirectory(uploadPath);
					}

					// Generate a unique file name
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
					string fullPath = Path.Combine(uploadPath, fileName);

					// Save the file to the server
					using (var stream = new FileStream(fullPath, FileMode.Create))
					{
						await image.CopyToAsync(stream);
					}

					// Generate the relative file URL
					string fileUrl = $"https://ebimtk-001-site1.mtempurl.com/uploads/{fileName}";

					// Save file information to the database
					var paymentForm = new PaymentForm
					{
						UserId = form.UserId,
						BankCard = form.BankCard,
						Month = form.Month,
						Year = form.Year,
						Status = "Pending",
						QueryType = form.QueryType,
						ImagePath = fileUrl
					};

					paymentForm.MonthlyPayment = _context.Users.SingleOrDefault(u => u.Id == paymentForm.UserId).SquareMeterSize * 0.05M;

					_context.PaymentForms.Add(paymentForm);
					await _context.SaveChangesAsync();

					return Ok(new { message = "Form successfully submitted!" });
				}
			}

			return BadRequest(ModelState);
		}



		[HttpGet("History/{userId}")]
		public async Task<ActionResult<IEnumerable<UserPaymentHistoryDTO>>> GetPaymentHistory(Guid userId)
		{
			if (!await _context.Users.AnyAsync(u => u.Id == userId))
			{
				return BadRequest("İstifadəçi mövcud deyil.");
			}

			var payments = await _context.PaymentForms
				.Where(u => u.UserId == userId)
				.Select(u => new UserPaymentHistoryDTO
				{
					PaymentDate = u.PaymentDate,
					Month = u.Month,
					Status = u.Status,
					ImagePath = u.ImagePath,
					MonthlyPayment = u.MonthlyPayment,
				}).OrderByDescending(p => p.PaymentDate)
				.ToListAsync();

			return Ok(payments);
		}


		[HttpGet("GetCurrentPayment")]
		public async Task<IActionResult> GetCurrentPayment(Guid userId)
		{
			var user = await _context.Users.FindAsync(userId);

			if (user is null)
			{
				return NotFound("İstifadəçi tapılmadı.");
			}

			return Ok(new { CurrentPayment = user.CurrentPayment });
		}


		/*// İstifadəçinin öz formunun statusunu izləməsi üçün
		// GET: api/payment/status/{id}
		[HttpGet("status/{id}")]
		public IActionResult GetFormStatus(int id)
		{
			var form = _context.PaymentForms.Find(id);
			if (form == null)
			{
				return NotFound();
			}

			return Ok(new { status = form.Status }); // Formun statusu qaytarılır
		}

		// Form məlumatlarını və şəkil yolunu adminə qaytar
		// GET: api/payment/form/{id}
		[HttpGet("form/{id}")]
		public IActionResult GetForm(int id)
		{
			var form = _context.PaymentForms.Find(id);
			if (form == null)
			{
				return NotFound();
			}

			// Form məlumatlarını və şəkil yolunu qaytar
			return Ok(new
			{
				form.BankCard,
				form.Month,
				form.Year,
				form.QueryType,
				form.Status,
				ImageUrl = form.ImagePath != null ? form.ImagePath : null // Şəkil varsa yolu qaytar
			});
		}*/
	}
}
