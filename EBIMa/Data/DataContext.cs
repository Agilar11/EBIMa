
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
		public DbSet<ApplicationRequest> ApplicationRequests { get; set; }
		public DbSet<ServiceSupply> ServiceSupplys { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>()
				.Property(u => u.CurrentPayment)
				.HasColumnType("decimal(18, 2)"); // 18 tam, 2 ondalık

			modelBuilder.Entity<PaymentForm>()
				.Property(p => p.MonthlyPayment)
				.HasColumnType("decimal(18, 2)"); // 18 tam, 2 ondalık
		}




	}
}
