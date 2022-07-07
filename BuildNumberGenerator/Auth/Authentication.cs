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
				context.Items[AuthIdentityKey] = new Identity()
				{
					Id = "E19A4322-1B0E-443E-B1A8-7F4D6FB7918C",
					Scopes = new[] { "getbuildnumber", "admin" }
				};
			}
		}

		private bool HasUseAuthenticationAttribute(HttpContext context)
		{
			var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
			return null != endpoint?.Metadata.GetMetadata<UseAuthenticationAttribute>();
		}

		private string? GetToken(HttpContext context)
		{
			context.Request.Headers.TryGetValue("Authorization", out var AuthorizationHeaders);
			var authorizationHeader = AuthorizationHeaders.FirstOrDefault();

			if (authorizationHeader is null || !authorizationHeader.StartsWith("Bearer "))
			{
				return null;
			}

			var cert = X509Certificate2.CreateFromPem("");

			return JwtBuilder.Create()
				.WithAlgorithm(new RS256Algorithm(cert))
				.WithValidationParameters(ValidationParameters.None)
				.Decode(authorizationHeader.Substring(7));
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
