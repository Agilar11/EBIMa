using Microsoft.Extensions.DependencyInjection;

namespace EBIMa.Services
{
	public static class ServiceExtensions
	{
		// Method to configure CORS
		public static void ConfigureCors(this IServiceCollection services)
		{
			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy", builder =>
				{
					builder.AllowAnyOrigin()  // Allows requests from any origin
						   .AllowAnyMethod()  // Allows any HTTP method (GET, POST, PUT, DELETE, etc.)
						   .AllowAnyHeader(); // Allows any header (Authorization, Content-Type, etc.)
				});
			});
		}
	}
}
