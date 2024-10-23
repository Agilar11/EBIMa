namespace EBIMa.Models
{
	public class ApplicationRequest
	{
		public int Id { get; set; }
		public string RequestType { get; set; } // e.g., "Şikayət", "Təklif", "Giriş kartı", "Digər"
		public string Message { get; set; }     // User's request message
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}

}
