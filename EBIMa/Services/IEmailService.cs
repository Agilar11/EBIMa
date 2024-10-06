using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace EBIMa.Services
{
	public interface IEmailService
	{
		void SendEmail(string to, string subject, string body);
	}

	public class EmailService : IEmailService
	{
		private readonly IConfiguration _configuration;

		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void SendEmail(string to, string subject, string body)
		{
			var smtpSettings = _configuration.GetSection("Smtp");

			if (!int.TryParse(smtpSettings["Port"], out int smtpPort))
			{
				throw new ArgumentException("SMTP Port is not configured properly.");
			}

			bool enableSSL = bool.TryParse(smtpSettings["EnableSSL"], out enableSSL) && enableSSL;

			string smtpHost = smtpSettings["Host"];
			string smtpUsername = smtpSettings["Username"];
			string smtpPassword = smtpSettings["Password"];

			if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
			{
				throw new ArgumentNullException("SMTP configuration is incomplete.");
			}

			var smtpClient = new SmtpClient(smtpHost)
			{
				Port = smtpPort,
				Credentials = new NetworkCredential(smtpUsername, smtpPassword),
				EnableSsl = enableSSL,
			};

			var mailMessage = new MailMessage
			{
				From = new MailAddress(smtpUsername),
				Subject = subject,
				Body = body,
				IsBodyHtml = true,
			};

			mailMessage.To.Add(to);

			smtpClient.Send(mailMessage);
		}

	}
}
