namespace EBIMa.Models
{
public class PaymentForm
	{
		public int Id { get; set; }
		public string BankCard { get; set; }
		public string Month { get; set; }
		public string Year { get; set; }
		public string QueryType { get; set; }
		public string Status { get; set; } = "Pending"; // İstifadəçinin sorğu statusu

		public string ImagePath { get; set; } // Yüklənmiş şəkilin serverdəki fayl yolu
	}



}
