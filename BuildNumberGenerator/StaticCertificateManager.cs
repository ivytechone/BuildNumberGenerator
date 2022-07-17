using System.Security.Cryptography.X509Certificates;

namespace BuildNumberGenerator
{
	/// <summary>
	/// A Certificate Manager that loads from PEM data in config
	/// </summary>
	public class StaticCertManager : ICertificateManager
	{
		private readonly StaticCertManagerConfig _config;

		public StaticCertManager(StaticCertManagerConfig config)
		{
			if (config == null || String.IsNullOrWhiteSpace(config.aidCertPem))
			{
				throw new Exception("StaticCertManagerConfig Missing");
			}

			_config = config;
		}

	public X509Certificate2 GetAidCertificate() => X509Certificate2.CreateFromPem(_config.aidCertPem);
	}
}