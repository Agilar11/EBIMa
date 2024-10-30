namespace EBIMa.DTO
{
	public class GetApplicationRequestsDTO
	{
		public int RequestId { get; set; }
		public string ApartmentNumber { get; set; } = string.Empty;
		public string? RequestType { get; set; } // e.g., "Şikayət", "Təklif", "Giriş kartı", "Digər"
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public string? Status { get; set; } = "Pending";
	}
}
