using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Archon.WebApi
{
	public static class LinkExtensions
	{
		public static async Task<TResponse> SendAsync<TLink, TResponse>(this HttpClient client, TLink link)
			where TLink : Link<TResponse>
		{
			if (link == null)
				throw new ArgumentNullException("link");

			var req = link.CreateRequest();

			var response = await client.SendAsync(req);

			return await link.ParseResponse(response);
		}
	}
}