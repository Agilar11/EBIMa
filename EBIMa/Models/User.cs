namespace EBIMa.Models
{
	public class User
	{
		public Guid Id { get; set; } = new Guid();
		public string Name { get; set; } = string.Empty;
		public string SurName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public byte[] PasswordHash { get; set; } = new byte[32];
		public byte[] PasswordSalt { get; set; } = new byte[32];
		public string? VerificationToken { get; set; }
		public DateTime? VerifiedAt { get; set; }
		public string? PasswordResetToken { get; set; }
		public DateTime? ResetTokenExpires { get; set; }


		// Apartment
		public string MTK { get; set; } = string.Empty;
		public string Building { get; set; } = string.Empty;
		public string? BlockNumber { get; set; } = string.Empty;
		public string? Floor { get; set; } = string.Empty;
		public string? ApartmentNumber { get; set; } = string.Empty;
		public string OwnerPhoneNumber { get; set; } = string.Empty;
		public string Role { get; set; } = "Resident"; // Default role is Resident

		public int SquareMeterSize { get; set; }

		public decimal CurrentPayment { get; set; } // Cari ödəniş məbləği
		public DateTime LastPaymentReset { get; set; } // Ödəniş sıfırlanma tarixi

		public ICollection<ApplicationRequest>? ApplicationRequests { get; set; }
		public ICollection<PaymentForm>? PaymentForms { get; set; }
	}
}
