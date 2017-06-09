using System.Net.Http;
using System.Threading.Tasks;

namespace Archon.WebApi.Tests.Links
{
	class LinkWithResponse : PlainLink, Link<string>
	{
		public async Task<string> ParseResponseAsync(HttpResponseMessage response)
		{
			return await response.Content.ReadAsStringAsync();
		}
	}

	class CustomLinkWithResponse : CustomLink, Link<string>
	{
		public CustomLinkWithResponse(string uri) : base(uri) { }

		public async Task<string> ParseResponseAsync(HttpResponseMessage response)
		{
			return await response.Content.ReadAsStringAsync();
		}
	}
}