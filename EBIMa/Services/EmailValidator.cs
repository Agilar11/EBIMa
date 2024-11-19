using DnsClient;

namespace EBIMa.Services
{
	public class EmailValidator
	{
		public async Task<bool> HasMxRecordsAsync(string email)
		{
			try
			{
				// E-posta adresinden alan adını ayır
				var domain = email.Split('@').Last();

				// DnsClient ile bir LookupClient oluştur
				var lookup = new LookupClient();

				// MX kayıtlarını sorgula
				var result = await lookup.QueryAsync(domain, QueryType.MX);
				var mxRecords = result.Answers.MxRecords();

				// Eğer MX kaydı varsa, alan adı e-posta alabilir
				return mxRecords.Any();
			}
			catch
			{
				// Herhangi bir hata durumunda false döndür
				return false;
			}
		}
	}
}
