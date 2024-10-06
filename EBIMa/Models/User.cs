namespace EBIMa.Models
{
	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty; // Ad (Name)
		public string Email { get; set; } = string.Empty;
		public byte[] PasswordHash { get; set; } = new byte[32];
		public byte[] PasswordSalt { get; set; } = new byte[32];
		public string? VerificationToken { get; set; }
		public DateTime? VerifiedAt { get; set; }
		public string? PasswordResetToken { get; set; }
		public DateTime? ResetTokenExpires { get; set; }

		// Additional fields based on the form
		public string MTK { get; set; } = string.Empty; // MTK of the apartment
		public string Building { get; set; } = string.Empty; // Building where the apartment is located
		public string BlockNumber { get; set; } = string.Empty; // Block number
		public string Floor { get; set; } = string.Empty; // Floor number (Mənzil Mərtəbəsi)
		public string ApartmentNumber { get; set; } = string.Empty; // Apartment number
		public string OwnerPhoneNumber { get; set; } = string.Empty; // Owner's phone number (Ev sahibinin nömrəsi)
	}

}
