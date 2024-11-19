namespace EBIMa.DTO
{
	public class UserPaymentHistoryDTO
	{
		public DateTime? PaymentDate { get; set; }
		public string? Month { get; set; }
		public decimal MonthlyPayment { get; set; }
		public string? Status { get; set; }
		public string? ImagePath { get; set; }
	}
}
