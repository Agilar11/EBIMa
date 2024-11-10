namespace EBIMa.DTO
{
	public class GetPaymentFormsDTO
	{
		public Guid Id { get; set; }
		public string? FullName { get; set; }
		public string? ApartmentNumber { get; set; }
		public string? Month { get; set; }
		public DateTime? PaymentDate { get; set; }
		public string? Status { get; set; }
		public string? ImagePath { get; set; }
	}
}
