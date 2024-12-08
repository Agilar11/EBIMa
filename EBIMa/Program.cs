// Global using directives
global using Microsoft.EntityFrameworkCore;
global using EBIMa.Models;
global using EBIMa.Data;
global using EBIMa.Services;
global using Microsoft.OpenApi.Models;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.IdentityModel.Tokens;
global using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Use Azure SQL Database connection string from appsettings.json
builder.Services.AddDbContext<DataContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register EmailService
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<PaymentResetService>();

// Read the JWT key from configuration
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
	throw new ArgumentNullException(nameof(jwtKey), "JWT key is not configured.");
}

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
		ValidateIssuer = false,
		ValidateAudience = false
	};
});

builder.Services.AddAuthorization();

// Add Swagger configuration
builder.Services.AddSwaggerGen(c =>
{
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Please enter token",
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey,
		BearerFormat = "JWT",
		Scheme = "Bearer"
	});
	c.AddSecurityRequirement(new OpenApiSecurityRequirement {
		{
			new OpenApiSecurityScheme {
				Reference = new OpenApiReference {
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			}, new string[] { }
		}
	});
});

// Add CORS policy
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend", policy =>
	{
		policy.WithOrigins("https://user.ebim.az", "http://localhost:5173") // Frontend URLs
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();