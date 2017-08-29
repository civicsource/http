using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Archon.Http.Tests
{
	class FakeHttpHandler : HttpMessageHandler
	{
		bool hasBeenCalled = false;

		public Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> Action { get; set; }

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			hasBeenCalled = true;

			if (Action == null)
				return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

			return Action(request, cancellationToken);
		}

		public void AssertHasBeenCalled()
		{
			if (!hasBeenCalled)
				throw new Exception("The fake http handler has not been called.");
		}
	}
}
