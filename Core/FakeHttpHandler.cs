using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace Archon.WebApi
{
	public class FakeHttpHandler : HttpMessageHandler
	{
		public Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> Action { get; set; }

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

			if (Action == null)
				return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

			return Action(request, cancellationToken);
		}
	}
}