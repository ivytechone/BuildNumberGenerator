using JWT;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Http.Features;
using System.Security.Cryptography.X509Certificates;

namespace BuildNumberGenerator
{
	public class UseAuthenticationAttribute : Attribute
	{
	}

	public class AuthenticationHelper
	{
		private readonly RequestDelegate _requestDelegate;

		public static readonly string AuthIdentityKey = "Auth_Identity";

		public AuthenticationHelper(RequestDelegate requestDelegate)
		{
			_requestDelegate = requestDelegate;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (!HasUseAuthenticationAttribute(context))
			{
				await _requestDelegate(context);
			}
			else
			{
				var token = GetToken(context);

				if (token is not null)
				{
					context.Items[AuthIdentityKey] = new Identity()
					{
						Id = token.sub,
						Scopes = Array.Empty<string>()
					};
					await _requestDelegate(context);
				}
				else
				{
					context.Response.StatusCode = 401;
					return;
				}
			}
		}

		private bool HasUseAuthenticationAttribute(HttpContext context)
		{
			var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
			return null != endpoint?.Metadata.GetMetadata<UseAuthenticationAttribute>();
		}

		private Token? GetToken(HttpContext context)
		{
			context.Request.Headers.TryGetValue("Authorization", out var AuthorizationHeaders);
			var authorizationHeader = AuthorizationHeaders.FirstOrDefault();

			if (authorizationHeader is null || !authorizationHeader.StartsWith("Bearer "))
			{
				return null;
			}

			var cert = X509Certificate2.CreateFromPem("-----BEGIN CERTIFICATE-----MIIDPzCCAiegAwIBAgIUWifwuXU9yzt6q60JysRzmtTUaywwDQYJKoZIhvcNAQELBQAwLzELMAkGA1UEBhMCVVMxCzAJBgNVBAgMAldBMRMwEQYDVQQKDApJdnlUZWNoT25lMB4XDTIyMDcwNTIzMzc1NVoXDTMyMDcwMjIzMzc1NVowLzELMAkGA1UEBhMCVVMxCzAJBgNVBAgMAldBMRMwEQYDVQQKDApJdnlUZWNoT25lMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA147qimzSX/F+SqqsAQvanerkULig/TNOH/4ytEzuf6MEFVk8YPckx1da3VtmyodD7aT2vDKqA/R8i2ehFswkgT+CKH7sTxuOp+DGsK3GAeIPS2NHodOq9spp9CuEVmZZGVwZNiRTvTp1iDr6bPkp5gDZ+9MaPsssyfKK3HJXq2bDT63v2UQ57jbZtfRgSZpTJZTsjJIoS+iAfzoNWbyNgf1oZy8jh5TqjY+bg26hLNPoB8q3ef5Opx6nG9kWaWfPrH5FR77yVv5eIMvzXhh3FfbLV9hvhyshwye28PjNV3E1qwKWd8/TS9/69exR/NVJYqYHAsjFy+TYv4NBN3BtUwIDAQABo1MwUTAdBgNVHQ4EFgQUYWKu/Cb9BSJVcG2jALmis+hYUygwHwYDVR0jBBgwFoAUYWKu/Cb9BSJVcG2jALmis+hYUygwDwYDVR0TAQH/BAUwAwEB/zANBgkqhkiG9w0BAQsFAAOCAQEAqnx0Ovul1KyM5YTMwtgv8489QawskajT1K8FoS+KBLLGZozg6rp4lzcZU2dq0p0Ue+AYTKEyKJtQM77a2C+dO9L2tHe06sAcQwSlL/pqw7c7p+Wzi9zm2SoCU20YwTnUtUb0fR9bRK4CRm4IhSwl4wpNQK43cTkFhLW3MOgNoo7GktjyHUeQEtpMUp9XsnZ1o/Ig5Y9VkrKDnU5Be+EG7SnyEnCFMmeeIsru4VSDdbNom73qb+UgKA7cAaKc/O05us2x2i2+QIaxUpjDhc4AqaAsGeQ73aOy5FR5BG1SfpDc9QoX9ASaOuWrRXkWLp52fAG4pWlaGhF+ey8tvJlLew==-----END CERTIFICATE-----");

			try
			{
				var token = JwtBuilder.Create()
					.WithAlgorithm(new RS256Algorithm(cert))
					.WithValidationParameters(ValidationParameters.Default)
					.Decode<Token>(authorizationHeader.Substring(7));
				
				if (token.iss == "ivytech.one" && token.aud == "2695BA2C-9C39-4D13-8AC3-B625A0963A19")
				{
					return token;
				}
				else
				{
					return null;
				}
			}
			catch (Exception)
			{
				return null;
			}

			
		}

		public static Identity? GetAuthenticatedIdentity(HttpContext context)
		{
			var data = context.Items[AuthIdentityKey];

			if (data is not null && data is Identity)
			{
				return (Identity?)data;
			}
			else
			{
				return null;
			}
		}
	}
}
