namespace EBIMa.Models
{
	public class ApplicationRequest
	{
		public int Id { get; set; }
		public int UserId { get; set; }  // Link to User ID
		public string? RequestType { get; set; } // e.g., "Şikayət", "Təklif", "Giriş kartı", "Digər"
		public string? Message { get; set; }     // User's request message
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public string? Status { get; set; } = "Pending";

		public User? User { get; set; } 
		
	}
}
