using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Archon.WebApi
{
	public abstract class AuthorizedLink : Link
	{
		public Authorization Authorization { get; set; }

		public HttpRequestMessage CreateRequest()
		{
			var req = CreateRequestInternal();

			if (req.Headers.Authorization == null)
			{
				var authToken = GetAuthToken();
				if (authToken != null)
					req.Headers.Authorization = authToken.AsHeader();
			}

			return req;
		}

		Authorization GetAuthToken()
		{
			if (Authorization != null)
				return Authorization;

			var claim = ClaimsPrincipal.Current.FindFirst("Token");

			if (claim != null && !String.IsNullOrWhiteSpace(claim.Value))
				return Authorization.Bearer(claim.Value);

			return null;
		}

		protected abstract HttpRequestMessage CreateRequestInternal();
	}

	public abstract class AuthorizedLink<TResponse> : AuthorizedLink, Link<TResponse>
	{
		public Task<TResponse> ParseResponseAsync(HttpResponseMessage response)
		{
			if (response == null)
				throw new ArgumentNullException("response");

			return ParseResponseInternal(response);
		}

		protected abstract Task<TResponse> ParseResponseInternal(HttpResponseMessage response);
	}
}