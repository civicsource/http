using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Archon.WebApi
{
	public abstract class Link
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

			var token = Thread.CurrentPrincipal as AuthToken;
			if (token != null)
				return Authorization.Bearer(token.Token);

			return null;
		}

		protected abstract HttpRequestMessage CreateRequestInternal();
	}

	public abstract class Link<TResponse> : Link
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