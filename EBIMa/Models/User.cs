namespace EBIMa.Models
{
	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public byte[] PasswordHash { get; set; } = new byte[32];
		public byte[] PasswordSalt { get; set; } = new byte[32];
		public string? VerificationToken { get; set; }
		public DateTime? VerifiedAt { get; set; }
		public string? PasswordResetToken { get; set; }
		public DateTime? ResetTokenExpires { get; set; }

		public string MTK { get; set; } = string.Empty;
		public string Building { get; set; } = string.Empty;
		public string BlockNumber { get; set; } = string.Empty;
		public string Floor { get; set; } = string.Empty;
		public string ApartmentNumber { get; set; } = string.Empty;
		public string OwnerPhoneNumber { get; set; } = string.Empty;

		public string Role { get; set; } = "Resident"; // Default role is Resident
	}
}
