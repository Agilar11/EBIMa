using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EBIMa.Services;
using EBIMa.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Google.Apis.PeopleService.v1;
using EBIMa.Models;

namespace EBIMa.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{

		


		private readonly DataContext _context;
		private readonly IEmailService _emailService;
		private readonly IConfiguration _configuration;
		private readonly ILogger<UserController> _logger;


		public UserController(DataContext context, IEmailService emailService, IConfiguration configuration, ILogger<UserController> logger)
		{
			_context = context;
			_emailService = emailService;
			_configuration = configuration;
			_logger = logger;

		}


		[HttpPost("Register")]
		public async Task<IActionResult> Register([FromBody] UserRegister userRegister)
		{
			var emailValidator = new EmailValidator();
			if (!await emailValidator.HasMxRecordsAsync(userRegister.Email))
			{
				return BadRequest("E-poçt ünvanı düzgün deyil və ya mövcud deyil.");
			}

			// Check if an unverified user with the same email exists
			var existingUser = await _context.Users
				.FirstOrDefaultAsync(u => u.Email == userRegister.Email);

			if (existingUser != null)
			{
				// If the email exists but the user is not verified, return a specific message
				if (existingUser.VerifiedAt == null)
				{
					return BadRequest("E-poçt ünvanı artıq qeydiyyatdan keçib, lakin təsdiqlənməyib. Zəhmət olmasa email təsdiqləyin.");
				}
				else
				{
					return BadRequest("İstifadəçi artıq mövcuddur.");
				}
			}

			var existingPhoneNumberUser = await _context.Users
		.FirstOrDefaultAsync(u => u.OwnerPhoneNumber == userRegister.OwnerPhoneNumber);

			if (existingPhoneNumberUser != null)
			{
				return BadRequest("Bu telefon nömrəsi artıq qeydiyyatdan keçib.");
			}

			// Create password hash and salt
			CreatePasswordHash(userRegister.Password, out byte[] passwordHash, out byte[] passwordSalt);

			// Create a new user object
			var user = new User
			{
				Name = userRegister.Name,
				SurName = userRegister.SurName,
				Email = userRegister.Email,
				PasswordHash = passwordHash,
				PasswordSalt = passwordSalt,
				MTK = userRegister.MTK,
				Building = userRegister.Building,
				BlockNumber = userRegister.BlockNumber,
				Floor = userRegister.Floor,
				ApartmentNumber = userRegister.ApartmentNumber,
				OwnerPhoneNumber = userRegister.OwnerPhoneNumber,
				Role = "Resident",
				SquareMeterSize = userRegister.SquareMeters,
				VerificationToken = CreateRandomToken(),
				VerificationTokenExpires = DateTime.UtcNow.AddHours(24) // 24 saatlıq limit
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			// Generate verification link using the token
			string verificationLink = $"https://user.ebim.az/verify?token={user.VerificationToken}";
			// Send email
			string subject = "Email təsdiqləmə";
			string body = $"Zəhmət olmasa hesabınızı təsdiqləmək üçün bu linkə klik edin: <a href='{verificationLink}'>Buraya Tıklayın</a>";

			_emailService.SendEmail(user.Email, subject, body);

			return Ok("User uğurla qeydiyyatdan keçdi. Email təsdiqləmə linki '"
					  + user.Email + "' ünvanına göndərildi.");
		}


		/*[HttpPost("ResendVerificationEmail")]
		public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendEmailRequest request)
		{
			// Find the user by email
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

			if (user == null)
			{
				return BadRequest("İstifadəçi mövcud deyil.");
			}

			// Check if the user is already verified
			if (user.VerifiedAt != null)
			{
				return BadRequest("E-poçt ünvanı artıq təsdiqlənib.");
			}

			// Generate a new verification token
			user.VerificationToken = CreateRandomToken();
			user.VerificationTokenExpires = DateTime.UtcNow.AddHours(24); // Reset the expiration time
			await _context.SaveChangesAsync();

			// Generate verification link using the token
			string verificationLink = $"https://user.ebim.az/verify?token={{user.VerificationToken}}\r\n={user.VerificationToken}";

			// Send email
			string subject = "Email təsdiqləmə (Yenidən göndərildi)";
			string body = $"Zəhmət olmasa hesabınızı təsdiqləmək üçün bu linkə klik edin: <a href='{verificationLink}'>Buraya Tıklayın</a>";

			_emailService.SendEmail(user.Email, subject, body);

			return Ok("Email təsdiqləmə linki yenidən göndərildi: " + user.Email);
		}*/


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

			string token = GenerateJwtToken(user);

			return Ok(new {UserId = user.Id,Role = user.Role, Token = token});
		}


		[HttpGet("verify")]
		public async Task<IActionResult> Verify([FromQuery] string token)
		{
			if (string.IsNullOrEmpty(token))
			{
				_logger.LogWarning("No token provided in the request.");
				return BadRequest("Token tələb olunur.");
			}

			var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);

			if (user == null)
			{
				_logger.LogWarning($"Invalid or expired token: {token}");
				return BadRequest("Token etibarsızdır və ya müddəti bitmişdir.");
			}

			if (user.VerifiedAt != null)
			{
				_logger.LogInformation($"User with email {user.Email} is already verified.");
				return BadRequest("İstifadəçi artıq təsdiqlənmişdir.");
			}

			// Verify the user
			user.VerifiedAt = DateTime.UtcNow;
			user.VerificationToken = null; // Optional: Clear token after successful verification
			await _context.SaveChangesAsync();

			_logger.LogInformation($"User with email {user.Email} successfully verified.");

			return Ok("Hesabınız uğurla təsdiqləndi!");
		}


		[HttpGet("{userId}")]
		public async Task<ActionResult<GetUserByIdDTO>> GetUserByIdAsync(Guid userId)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
			
			if(user.VerifiedAt is null)
			{
				return BadRequest("İstifadəçi girişi təsdiqlənməyib.");
			}

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

		[HttpPut("{userId}")]
		public async Task<IActionResult> UpdateUserAsync(Guid userId,[FromBody] UpdateUserDTO newUser)
		{
			if(string.IsNullOrEmpty(newUser.Name) || string.IsNullOrEmpty(newUser.Surname) 
				|| string.IsNullOrEmpty(newUser.Email) || string.IsNullOrEmpty(newUser.PhoneNumber))
			{
				return BadRequest("Məlumatlarınızı düzgün daxil edin.");
			}

			var user = await _context.Users.FindAsync(userId);

			if (user.VerifiedAt is null)
			{
				return BadRequest("İstifadəçi girişi təsdiqlənməyib.");
			}

			if (user is null)
			{
				return BadRequest("İstifadəçi mövcud deyil.");
			}

			user.Name = newUser.Name;
			user.SurName = newUser.Surname;
			user.Email = newUser.Email;
			user.OwnerPhoneNumber = newUser.PhoneNumber;

			await _context.SaveChangesAsync();

			return Ok("Məlumatlarınız uğurla yeniləndi.");

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
			//var resetLink = Url.Action("ResetPassword", "User", new { token = user.PasswordResetToken }, Request.Scheme);
			var resetLink = $"https://user.ebim.az/resetpassword?token={user.PasswordResetToken}";
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



		[HttpPost("logout")]
		public  IActionResult Logout()
		{
			return Ok("İstifadəçi uğurla çıxış etdi.");
		}

		
		
		// Method to create a random token for verification
		private string CreateRandomToken()
		{
			return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
		}

		private string GenerateJwtToken(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.Role, user.Role)
				}),
				Expires = DateTime.UtcNow.AddHours(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

	}
}
