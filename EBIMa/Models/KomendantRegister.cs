using System.ComponentModel.DataAnnotations;
using DnsClient.Protocol;
using DnsClient;

namespace EBIMa.Models
{
	public class KomendantRegister
	{
		[Required(ErrorMessage = "Ad sahəsi tələb olunur.")]
		[StringLength(50, ErrorMessage = "Ad ən çox 50 simvol ola bilər.")]
		public string Name { get; set; } = string.Empty;

		[Required(ErrorMessage = "Soyad sahəsi tələb olunur.")]
		[StringLength(50, ErrorMessage = "Soyad ən çox 50 simvol ola bilər.")]
		public string SurName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Email sahəsi tələb olunur.")]
		[EmailAddress(ErrorMessage = "Düzgün email daxil edin.")]
		[StringLength(256, ErrorMessage = "Email ən çox 256 simvoldan ibarət ola bilər.")]
		[CustomValidation(typeof(UserRegister), nameof(ValidateDomain))]
		public string Email { get; set; } = string.Empty;


		[Required(ErrorMessage = "Şifrə sahəsi tələb olunur.")]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "Şifrə ən az 6 simvol olmalıdır.")]
		public string Password { get; set; } = string.Empty;

		[Required(ErrorMessage = "MTK sahəsi tələb olunur.")]
		public string MTK { get; set; } = string.Empty;


		[Required(ErrorMessage = "Ev sahibinin nömrəsi tələb olunur.")]
		[Phone(ErrorMessage = "Düzgün telefon nömrəsi daxil edin.")]
		[RegularExpression(@"^\+994\d{9}$", ErrorMessage = "Telefon nömrəsi +994 ilə başlamalı və 9 rəqəmli olmalıdır.")]
		public string OwnerPhoneNumber { get; set; } = string.Empty;


		
		public static ValidationResult ValidateDomain(object value, ValidationContext context)
		{
			if (value is string email)
			{
				// Emaili '@' işarəsinə görə bölüb domaini əldə edirik
				var domain = email.Split('@')[1];

				// Domen üçün MX qeydinin olub-olmadığını yoxlayırıq
				if (!HasMXRecord(domain))
				{
					return new ValidationResult("Bu domen email qəbul etmir və ya mövcud deyil.");
				}
			}
			else
			{
				return new ValidationResult("Email formatı düzgün deyil.");
			}

			return ValidationResult.Success;
		}

		private static bool HasMXRecord(string domain)
		{
			try
			{
				var lookup = new LookupClient();
				var result = lookup.Query(domain, QueryType.MX);

				// MX qeydlərini yoxlamaq üçün fərqli bir metoddan istifadə
				foreach (var record in result.Answers)
				{
					if (record is MxRecord)
					{
						return true;
					}
				}
				return false;
			}
			catch
			{
				return false;
			}
		}
	}
}
