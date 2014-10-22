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
}