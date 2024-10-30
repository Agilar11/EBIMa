namespace EBIMa.DTO
{
	public class GetApplicationRequestsByIdDTO
	{
		public string ApartmentNumber { get; set; } = string.Empty;
		public string? RequestType { get; set; } // e.g., "Şikayət", "Təklif", "Giriş kartı", "Digər"
		public string? Message { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public string? Status { get; set; } = "Pending";
	}
}
