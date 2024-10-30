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

		// POST: api/payment/submit
		[HttpPost("submit")]
		public async Task<IActionResult> SubmitForm([FromForm] SubmitFormDTO form, IFormFile image)
		{
			var userId = int.Parse(User.Identity.Name);

			if (ModelState.IsValid)
			{
				// Azure Blob Storage connection string
				string connectionString = Environment.GetEnvironmentVariable("azureconnectionstring");
				string containerName = "upload"; // Yüklənəcək konteyner adı

				if (image != null && image.Length > 0)
				{
					// Blob Container ilə əlaqə qurmaq
					BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
					await containerClient.CreateIfNotExistsAsync();

					// Fayl adını təyin edin (özelleştirə bilərsiniz)
					string blobName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

					// Blob client yaratmaq
					BlobClient blobClient = containerClient.GetBlobClient(blobName);

					// Faylı yükləmək
					using (var stream = image.OpenReadStream())
					{
						await blobClient.UploadAsync(stream);
					}

					// Faylın URL-ni əldə edin və bazaya qeyd edin
					string blobUrl = blobClient.Uri.ToString();

					var paymentForm = new PaymentForm
					{
						UserId = userId,
						BankCard = form.BankCard,
						Month = form.Month,
						Year = form.Year,
						Status = "Pending",
						QueryType = form.QueryType,
						ImagePath = blobUrl
					};

					_context.PaymentForms.Add(paymentForm);
					_context.SaveChanges();

					return Ok(new { message = "Form uğurla göndərildi!" });
				}
			}

			return BadRequest(ModelState);
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
				ImageUrl = form.ImagePath != null ? form.ImagePath : null // Şəkil varsa yolu qaytar
			});
		}
	}
}
