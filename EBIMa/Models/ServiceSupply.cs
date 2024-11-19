using System.ComponentModel.DataAnnotations;

namespace EBIMa.Models
{
	public class ServiceSupply
	{
		public Guid Id { get; set; }

		[Required(ErrorMessage = "Ad sahəsi tələb olunur.")]
		public string? Name { get; set; } = string.Empty;

		[Required(ErrorMessage = "Soyad sahəsi tələb olunur.")]
		public string? Surname { get; set; } = string.Empty;

		[Required(ErrorMessage = "Vəzifə sahəsi tələb olunur.")]
		public string? Profession { get; set; } = string.Empty;

		[Required(ErrorMessage = "İşçinin nömrəsi tələb olunur.")]
		[Phone(ErrorMessage = "Düzgün telefon nömrəsi daxil edin.")]
		[RegularExpression(@"^\+994\d{9}$", ErrorMessage = "Telefon nömrəsi +994 ilə başlamalı və 9 rəqəmli olmalıdır.")]
		public string? PhoneNumber { get; set; } = string.Empty;
	}
}
