using System.Net.Http;

namespace Archon.WebApi.Tests.Links
{
	class PlainLink : Link
	{
		public HttpRequestMessage CreateRequest()
		{
			return new HttpRequestMessage(HttpMethod.Get, "http://example.com/");
		}
	}

	class CustomLink : Link
	{
		private string uri;
		public CustomLink(string uri)
		{
			this.uri = uri;
		}
		public HttpRequestMessage CreateRequest()
		{
			return new HttpRequestMessage(HttpMethod.Get, $"http://example.com/{uri}");
		}
	}
}