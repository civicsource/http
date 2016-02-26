using System;
using System.Net.Http;

namespace Archon.WebApi
{
	public static class LinkExtensions
	{
		public static TResponse ParseResponse<TResponse>(this Link<TResponse> link, HttpResponseMessage response)
		{
			if (link == null)
				throw new ArgumentNullException("link");

			return link.ParseResponseAsync(response).Result;
		}
	}
}