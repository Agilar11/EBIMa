using System.ComponentModel.DataAnnotations;

namespace EBIMa.Models
{
	public class UserLogin
	{

		[Required(ErrorMessage = "Email sahəsi tələb olunur.")]
		[EmailAddress(ErrorMessage = "Düzgün email daxil edin.")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Şifrə sahəsi tələb olunur.")]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "Şifrə ən az 6 simvol olmalıdır.")]

		public string Password { get; set; } = string.Empty; // Password input during registration




		   
	}
}
