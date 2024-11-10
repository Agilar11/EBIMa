namespace EBIMa.Services
{
	public class PaymentResetService : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;

		public PaymentResetService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var now = DateTime.Now;
				if (now.Day == 10 && now.Hour == 0) // Hər ayın 1-də, gecə yarısı
				{
					using (var scope = _serviceProvider.CreateScope())
					{
						var context = scope.ServiceProvider.GetRequiredService<DataContext>();
						var users = await context.Users.ToListAsync(stoppingToken);

						foreach (var user in users)
						{
							if (user.LastPaymentReset.Month < now.Month || user.LastPaymentReset.Year < now.Year)
							{
								user.CurrentPayment = user.SquareMeterSize * 0.05M;
								user.LastPaymentReset = now;
							}
						}

						await context.SaveChangesAsync(stoppingToken);
					}

					await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Bir gün gözləyir
				}
				else
				{
					await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Hər saat yoxlayır
				}
			}
		}

	}
}
