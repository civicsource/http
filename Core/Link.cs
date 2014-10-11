using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Archon.WebApi
{
	public abstract class Link
	{
		public RequestAuthentication Authorization { get; set; }

		public HttpRequestMessage CreateRequest()
		{
			var req = CreateRequestInternal();

			if (Authorization != null)
				req.Headers.Authorization = Authorization.AsHeader();

			return req;
		}

		protected abstract HttpRequestMessage CreateRequestInternal();
	}

	public abstract class Link<TResponse> : Link
	{
		public Task<TResponse> ParseResponse(HttpResponseMessage response)
		{
			if (response == null)
				throw new ArgumentNullException("response");

			return ParseResponseInternal(response);
		}

		protected abstract Task<TResponse> ParseResponseInternal(HttpResponseMessage response);
	}
}