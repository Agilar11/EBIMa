namespace EBIMa.Models
{
	public class ResidentRequest
	{
		public Guid Id { get; set; } = new Guid();
		public Guid ResidentId { get; set; } // The resident making the request
		public User Resident { get; set; } = default!;

		public string RequestDetails { get; set; } = string.Empty; // Details of the request

		public DateTime CreatedAt { get; set; } = DateTime.Now; // Request creation time
		public bool? IsApproved { get; set; } // Null until approved/denied, true = approved, false = denied
	}
}
