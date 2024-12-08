using System.ComponentModel.DataAnnotations;

namespace EBIMa.DTO
{
	public class ServiceSupplyDTO
	{
		[Required(ErrorMessage = "Ad sahəsi tələb olunur.")]
		public string? Name { get; set; } = string.Empty;

		[Required(ErrorMessage = "Soyad sahəsi tələb olunur.")]
		public string? Surname { get; set; } = string.Empty;

		[Required(ErrorMessage = "Vəzifə sahəsi tələb olunur.")]
		public string? Profession { get; set; } = string.Empty;

		[Required(ErrorMessage = "Ev sahibinin nömrəsi tələb olunur.")]
		[Phone(ErrorMessage = "Düzgün telefon nömrəsi daxil edin.")]
		[RegularExpression(@"^\+994\d{9}$", ErrorMessage = "Telefon nömrəsi +994 ilə başlamalı və 9 rəqəmli olmalıdır.")]
		public string? PhoneNumber { get; set; }
	}
}
