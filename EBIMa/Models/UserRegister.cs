using System.ComponentModel.DataAnnotations;

namespace EBIMa.Models
{
	public class UserRegister
	{
		[Required(ErrorMessage = "Ad sahəsi tələb olunur.")]
		[StringLength(50, ErrorMessage = "Ad ən çox 50 simvol ola bilər.")]
		public string Name { get; set; } = string.Empty;

		[Required(ErrorMessage = "Email sahəsi tələb olunur.")]
		[EmailAddress(ErrorMessage = "Düzgün email daxil edin.")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Şifrə sahəsi tələb olunur.")]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "Şifrə ən az 6 simvol olmalıdır.")]
		public string Password { get; set; } = string.Empty;

		[Required(ErrorMessage = "MTK sahəsi tələb olunur.")]
		public string MTK { get; set; } = string.Empty;

		[Required(ErrorMessage = "Bina sahəsi tələb olunur.")]
		public string Building { get; set; } = string.Empty;

		[Required(ErrorMessage = "Blok nömrəsi sahəsi tələb olunur.")]
		public string BlockNumber { get; set; } = string.Empty;

		[Required(ErrorMessage = "Mərtəbə nömrəsi sahəsi tələb olunur.")]
		public string Floor { get; set; } = string.Empty;

		[Required(ErrorMessage = "Mənzil nömrəsi sahəsi tələb olunur.")]
		public string ApartmentNumber { get; set; } = string.Empty;

		[Required(ErrorMessage = "Ev sahibinin nömrəsi tələb olunur.")]
		[Phone(ErrorMessage = "Düzgün telefon nömrəsi daxil edin.")]
		public string OwnerPhoneNumber { get; set; } = string.Empty;

		public string Role { get; set; } = "Resident"; // Allow selecting role during registration
	}
}
