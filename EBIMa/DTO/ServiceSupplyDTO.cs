using System.ComponentModel.DataAnnotations;

namespace EBIMa.DTO
{
	public class ServiceSupplyDTO
	{
		public string? Name { get; set; }
		public string? Surname { get; set; }
		public string? Profession { get; set; }

		[RegularExpression(@"^\+994\d{9}$", ErrorMessage = "Telefon nömrəsi +994 ilə başlamalı və 9 rəqəmli olmalıdır.")]
		public string? PhoneNumber { get; set; }
	}
}
