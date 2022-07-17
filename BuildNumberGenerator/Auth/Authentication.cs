using IvyTech.Logging;
using Microsoft.AspNetCore.Http.Features;
using JWT;
using JWT.Algorithms;
using JWT.Builder;

namespace BuildNumberGenerator
{
	public class UseAuthenticationAttribute : Attribute
	{
	}

	public class AuthenticationHelper
	{
		private readonly RequestDelegate _requestDelegate;
		private readonly ILogger<AuthenticationHelper> _logger;
		private readonly ICertificateManager _certManager;
		public static readonly string AuthIdentityKey = "Auth_Identity";


		public AuthenticationHelper(RequestDelegate requestDelegate, ILogger<AuthenticationHelper> logger, ICertificateManager certManager)
		{
			_requestDelegate = requestDelegate;
			_logger = logger;
			_certManager = certManager;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var requestLoggerContext = context.GetRequestLoggerContext();

			if (!HasUseAuthenticationAttribute(context))
			{
				await _requestDelegate(context);
			}
			else
			{
				var token = GetToken(context);

				if (token is not null)
				{
					if (token.sub is null || token.zoneinfo is null)
					{
						_logger.LogWarning("Token not valid");
						requestLoggerContext.Diag = DiagCodes.AuthFailed;
						context.Response.StatusCode = 401;
						return;
					}
					
					context.Items[AuthIdentityKey] = new Identity()
					{
						Id = token.sub,
						Scopes = Array.Empty<string>(),
						TZ = TimeZoneInfo.FindSystemTimeZoneById(token.zoneinfo)
					};
					await _requestDelegate(context);
				}
				else
				{
					_logger.LogWarning("Token missing or invalid");
					requestLoggerContext.Diag = DiagCodes.AuthFailed;		
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

			var cert = _certManager.GetAidCertificate();

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
