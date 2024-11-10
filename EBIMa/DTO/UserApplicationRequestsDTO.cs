namespace EBIMa.DTO
{
	public class UserApplicationRequestsDTO
	{
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public string? RequestType { get; set; } // e.g., "Şikayət", "Təklif", "Giriş kartı", "Digər"
		public string? Status { get; set; } = "Pending";
	}
}
