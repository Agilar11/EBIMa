namespace EBIMa.Models
{
public class PaymentForm
	{
		public Guid Id { get; set; } = new Guid();
		public Guid UserId { get; set; }
		public string? BankCard { get; set; } = "0000-0000-0000-0000";
		public string? Month { get; set; }
		public string? Year { get; set; }
		public DateTime? PaymentDate { get; set; } = DateTime.Now; // add
		public string? QueryType { get; set; }
		public string? Status { get; set; } = "Pending"; // İstifadəçinin sorğu statusu
		public string? ImagePath { get; set; } // Yüklənmiş şəkilin serverdəki fayl yolu
		public decimal MonthlyPayment { get; set; }

		public User? User { get; set; }

	}
}
