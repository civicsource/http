using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace Archon.Http
{
	/// <remarks>
	/// http://benfoster.io/blog/adding-patch-support-to-httpclient
	/// </remarks>
	public static class HttpPatchClientSupport
	{
		public static Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient client, string requestUri, T value)
		{
			if (client == null)
				throw new ArgumentNullException("client");

			if (string.IsNullOrWhiteSpace(requestUri))
				throw new ArgumentNullException("requestUri");

			if (value == null)
				throw new ArgumentNullException("value");

			var content = new ObjectContent<T>(value, new JsonMediaTypeFormatter());
			var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri) { Content = content };

			return client.SendAsync(request);
		}
	}
}