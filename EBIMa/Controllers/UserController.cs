using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EBIMa.Services;

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

		// Method to create a random token for verification
		private string CreateRandomToken()
		{
			return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
		}
	}
}
