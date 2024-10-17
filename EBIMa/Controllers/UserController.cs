using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EBIMa.Services;
using EBIMa.DTO;

namespace EBIMa.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly DataContext _context;
		private readonly IEmailService _emailService;

		public UserController(DataContext context, IEmailService emailService)
		{
			_context = context;
			_emailService = emailService;
		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register([FromBody] UserRegister userRegister)
		{
			// Check if the user already exists asynchronously
			if (await _context.Users.AnyAsync(u => u.Email == userRegister.Email))
			{
				return BadRequest("İstifadəçi artıq mövcuddur.");
			}

			// Create password hash and salt
			CreatePasswordHash(userRegister.Password, out byte[] passwordHash, out byte[] passwordSalt);

			// Create a new user object
			var user = new User
			{
				Name = userRegister.Name,
				Email = userRegister.Email,
				PasswordHash = passwordHash,
				PasswordSalt = passwordSalt,
				MTK = userRegister.MTK,
				Building = userRegister.Building,
				BlockNumber = userRegister.BlockNumber,
				Floor = userRegister.Floor,
				ApartmentNumber = userRegister.ApartmentNumber,
				OwnerPhoneNumber = userRegister.OwnerPhoneNumber,
				Role = userRegister.Role,  // Role set based on registration
				VerificationToken = CreateRandomToken(),
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			// Generate verification link using the token
			var verificationLink = Url.Action("Verify", "User", new { token = user.VerificationToken }, Request.Scheme);

			// Send email
			string subject = "Email təsdiqləmə";
			string body = $"Zəhmət olmasa hesabınızı təsdiqləmək üçün bu linkə klik edin: <a href='{verificationLink}'>Buraya Tıklayın</a>";

			_emailService.SendEmail(user.Email, subject, body);

			return Ok("İstifadəçi uğurla qeydiyyatdan keçdi. Email təsdiqləmə linki göndərildi.");
		}

		[HttpPost("Login")]
		public async Task<IActionResult> UserLogin([FromBody] UserLogin userLogin)
		{
			// Retrieve the user by email asynchronously
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userLogin.Email);

			// Check if the user exists
			if (user == null)
			{
				return BadRequest("İstifadəçi mövcud deyil.");
			}

			// Check if the user is verified
			if (user.VerifiedAt == null)
			{
				return BadRequest("İstifadəçi təsdiq olunmayıb.");
			}

			// Verify the password
			if (!VerifyPasswordHash(userLogin.Password, user.PasswordHash, user.PasswordSalt))
			{
				return BadRequest("Yanlış parol.");
			}

			return Ok($"Xoş gəldiniz, {user.Email}! :)");
		}

		[HttpGet("verify")]
		public async Task<IActionResult> Verify(string token)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
			if (user == null)
			{
				return BadRequest("Invalid token.");
			}

			user.VerifiedAt = DateTime.Now;
			await _context.SaveChangesAsync();

			return Ok("User verified! :)");
		}


		// Method to verify password hash
		private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512(passwordSalt))
			{
				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				return computedHash.SequenceEqual(passwordHash);
			}
		}

		// Method to create password hash and salt
		private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}


		// Forgot Password
		[HttpPost("ForgotPassword")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
		{
			// Check if the user exists
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
			if (user == null)
			{
				return BadRequest("İstifadəçi mövcud deyil.");
			}

			// Generate a reset token
			user.PasswordResetToken = CreateRandomToken();
			user.ResetTokenExpires = DateTime.Now.AddHours(1); // Token is valid for 1 hour
			await _context.SaveChangesAsync();

			// Send reset email
			var resetLink = Url.Action("ResetPassword", "User", new { token = user.PasswordResetToken }, Request.Scheme);
			string subject = "Parolun sıfırlanması";
			string body = $"Zəhmət olmasa yeni parol təyin etmək üçün bu linkə klik edin: <a href='{resetLink}'>Parolu sıfırla</a>";

			_emailService.SendEmail(user.Email, subject, body);

			return Ok("Parolu sıfırlamaq üçün link email ünvanınıza göndərildi.");
		}

		// Reset Password
		[HttpPost("ResetPassword")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
		{
			// Find the user by reset token
			var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);
			if (user == null || user.ResetTokenExpires < DateTime.Now)
			{
				return BadRequest("Yanlış və ya vaxtı bitmiş token.");
			}

			// Create new password hash and salt
			CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

			// Update user's password
			user.PasswordHash = passwordHash;
			user.PasswordSalt = passwordSalt;
			user.PasswordResetToken = null; // Clear the token after successful reset
			user.ResetTokenExpires = null;
			await _context.SaveChangesAsync();

			return Ok("Parol uğurla yeniləndi.");
		}






		[HttpPost("SubmitRequest")]
		public async Task<IActionResult> SubmitRequest([FromBody] ResidentRequest residentRequest)
		{
			// Fetch the resident from the database
			var resident = await _context.Users.FirstOrDefaultAsync(u => u.Id == residentRequest.ResidentId && u.Role == "Resident");
			if (resident == null)
			{
				return BadRequest("Resident not found.");
			}

			// Create the request
			var request = new ResidentRequest
			{
				ResidentId = resident.Id,
				RequestDetails = residentRequest.RequestDetails
			};

			_context.ResidentRequests.Add(request);
			await _context.SaveChangesAsync();

			return Ok("Request submitted successfully.");
		}




		// Method to create a random token for verification
		private string CreateRandomToken()
		{
			return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
		}
	}
}
