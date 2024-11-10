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

		[Phone(ErrorMessage = "Düzgün telefon nömrəsi daxil edin.")]
		public string? PhoneNumber { get; set; } = string.Empty;
	}
}
