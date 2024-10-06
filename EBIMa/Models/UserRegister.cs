using System.ComponentModel.DataAnnotations;

namespace EBIMa.Models
{
	public class UserRegister
	{
		[Required(ErrorMessage = "Ad sahəsi tələb olunur.")]
		[StringLength(50, ErrorMessage = "Ad ən çox 50 simvol ola bilər.")]
		public string Name { get; set; } = string.Empty; // Ad (Name)

		[Required(ErrorMessage = "Email sahəsi tələb olunur.")]
		[EmailAddress(ErrorMessage = "Düzgün email daxil edin.")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Şifrə sahəsi tələb olunur.")]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "Şifrə ən az 6 simvol olmalıdır.")]
		public string Password { get; set; } = string.Empty; // Password input during registration

		[Required(ErrorMessage = "MTK sahəsi tələb olunur.")]
		public string MTK { get; set; } = string.Empty; // MTK of the apartment

		[Required(ErrorMessage = "Bina sahəsi tələb olunur.")]
		public string Building { get; set; } = string.Empty; // Building where the apartment is located

		[Required(ErrorMessage = "Blok nömrəsi sahəsi tələb olunur.")]
		public string BlockNumber { get; set; } = string.Empty; // Block number

		[Required(ErrorMessage = "Mərtəbə nömrəsi sahəsi tələb olunur.")]
		public string Floor { get; set; } = string.Empty; // Floor number (Mənzil Mərtəbəsi)

		[Required(ErrorMessage = "Mənzil nömrəsi sahəsi tələb olunur.")]
		public string ApartmentNumber { get; set; } = string.Empty; // Apartment number

		[Required(ErrorMessage = "Ev sahibinin nömrəsi tələb olunur.")]
		[Phone(ErrorMessage = "Düzgün telefon nömrəsi daxil edin.")]
		public string OwnerPhoneNumber { get; set; } = string.Empty; // Owner's phone number (Ev sahibinin nömrəsi)
	}
}
