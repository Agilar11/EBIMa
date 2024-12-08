using System.ComponentModel.DataAnnotations;

namespace EBIMa.Models
{
	public class UserLogin
	{

		[Required(ErrorMessage = "Email sahəsi tələb olunur.")]
		[EmailAddress(ErrorMessage = "Düzgün email daxil edin.")]
		public string Email { get; set; } = string.Empty;


		[Required(ErrorMessage = "Şifrə sahəsi tələb olunur.")]
		[StringLength(100, MinimumLength = 8, ErrorMessage = "Şifrə ən az 8 simvol olmalıdır.")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!()%*?&#])[A-Za-z\d@$!()%*?&#]{8,}$",
		ErrorMessage = "Şifrə ən az 1 böyük hərf, 1 kiçik hərf, 1 rəqəm və 1 xüsusi simvol içərməlidir.")]

		public string Password { get; set; } = string.Empty; // Password input during registration






	}
}
