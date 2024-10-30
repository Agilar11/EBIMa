namespace EBIMa.DTO
{
	public class GetUserByIdDTO
	{
		public string? Name { get; set; }
		public string? Surname { get; set; }
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
		public string? MTK { get; set; }
		public string? BlockNumber { get; set; }
		public string? Floor { get; set; }
		public string? ApartmentNumber { get; set; }
		public int SquareMeters { get; set; }

		// Monthly payment calculation based on square meters (0.05 AZN per m²)
		public decimal MonthlyPayment { get; set; } // set özəlliyi əlavə edildi


	}
}
