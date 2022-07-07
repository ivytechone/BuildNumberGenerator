using Microsoft.AspNetCore.Http.Features;

namespace BuildNumberGenerator
{
	public class UseAuthorizationAttribute : Attribute
	{
	}

	public class AuthorizationHelper
	{
		private readonly RequestDelegate _requestDelegate;

		public AuthorizationHelper(RequestDelegate requestDelegate)
		{
			_requestDelegate = requestDelegate;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
			if (null != endpoint?.Metadata.GetMetadata<UseAuthenticationAttribute>())
			{
				var Identity = AuthenticationHelper.GetAuthenticatedIdentity(context);

				if (Identity is not null &&
					!string.IsNullOrEmpty(Identity.Id) &&
					Identity.Scopes is not null &&
					Identity.Scopes.Contains("admin"))
				{
					await _requestDelegate(context);
				}
				else
				{
					context.Response.StatusCode = 401;
					return;
				}
			}
			else
			{
				await _requestDelegate(context);
			}
		}
	}
}
