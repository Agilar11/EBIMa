namespace EBIMa.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{
		}

		public DbSet<User> Users => Set<User>();
		public DbSet<ResidentRequest> ResidentRequests => Set<ResidentRequest>(); // Add ResidentRequests to DataContext

		public DbSet<PaymentForm> PaymentForms { get; set; }



	}
}
