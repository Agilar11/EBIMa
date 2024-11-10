namespace EBIMa.DTO
{
	public class SubmitFormDTO
	{
		public Guid UserId { get; set; }
		public string? BankCard { get; set; }
		public string? Month { get; set; }
		public string? Year { get; set; }
		public string? QueryType { get; set; }

	}
}
