using System;
using System.Collections.Generic;
using System.Linq;
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
		bool hasBeenCalled = false;

		public Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> Action
		{
			get
			{
				return actions.LastOrDefault();
			}
			set
			{
				actions.Add(value);
			}
		}

		private List<Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>> actions = new List<Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>>();
		public IEnumerable<Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>> Actions
		{
			get
			{
				return actions;
			}
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

			hasBeenCalled = true;

			foreach (var ac in Actions)
			{
				var response = ac(request, cancellationToken);
				if (response != null)
					return response;
			}

			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
		}

		public void AssertHasBeenCalled()
		{
			if (!hasBeenCalled)
				throw new Exception("The fake http handler has not been called.");
		}
	}
}