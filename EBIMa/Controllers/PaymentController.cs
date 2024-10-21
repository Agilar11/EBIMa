using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace EBIMa.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PaymentController : ControllerBase
	{
		private readonly DataContext _context;

		public PaymentController(DataContext context)
		{
			_context = context;
		}

		// POST: api/payment/submit
		[HttpPost("submit")]
		public IActionResult SubmitForm([FromForm] PaymentForm form, [FromForm] IFormFile image)
		{
			if (ModelState.IsValid)
			{
				if (image != null)
				{
					var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
					var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;

					// Faylı serverə yüklə
					var filePath = Path.Combine(uploadsFolder, uniqueFileName);
					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						image.CopyTo(fileStream);
					}

					form.ImagePath = uniqueFileName;
				}

				form.Status = "Pending"; 
				_context.PaymentForms.Add(form); 
				_context.SaveChanges(); 

				return Ok(new { message = "Form uğurla göndərildi!" });
			}

			return BadRequest(ModelState);
		}

		[HttpPost("approve/{id}")]
		public IActionResult ApproveForm(int id)
		{
			var form = _context.PaymentForms.Find(id);
			if (form == null)
			{
				return NotFound();
			}

			form.Status = "Approved"; 
			_context.SaveChanges();

			return Ok(new { message = "Form təsdiq olundu!" });
		}

		[HttpPost("deny/{id}")]
		public IActionResult DenyForm(int id)
		{
			var form = _context.PaymentForms.Find(id);
			if (form == null)
			{
				return NotFound();
			}

			form.Status = "Denied"; // Form rədd edildi
			_context.SaveChanges();

			return Ok(new { message = "Form rədd edildi!" });
		}

		// İstifadəçinin öz formunun statusunu izləməsi üçün
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
				ImageUrl = form.ImagePath != null ? $"/images/{form.ImagePath}" : null // Şəkil varsa yolu qaytar
			});
		}
	}
}
