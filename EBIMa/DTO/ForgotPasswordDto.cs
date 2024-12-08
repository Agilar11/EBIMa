using System.ComponentModel.DataAnnotations;

namespace EBIMa.DTO
{
	public class ForgotPasswordDto
	{
		public string? Email { get; set; }
	}

	public class ResetPasswordDto
	{
		public string? Token { get; set; }

		[Required(ErrorMessage = "Şifrə sahəsi tələb olunur.")]
		[StringLength(100, MinimumLength = 8, ErrorMessage = "Şifrə ən az 8 simvol olmalıdır.")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$()!%*?&#])[A-Za-z\d@$!()%*?&#]{8,}$",
		ErrorMessage = "Şifrə ən az 1 böyük hərf, 1 kiçik hərf, 1 rəqəm və 1 xüsusi simvol içərməlidir.")]
		public string? NewPassword { get; set; }
	}

}
