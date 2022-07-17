using System.Security.Cryptography.X509Certificates;

namespace BuildNumberGenerator {
	public interface ICertificateManager
	{
		X509Certificate2 GetAidCertificate();
	}
}