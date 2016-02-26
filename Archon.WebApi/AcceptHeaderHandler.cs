using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Archon.WebApi
{
	public class AcceptHeaderHandler : DelegatingHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var query = request.GetQueryNameValuePairs();
			var accept = query.FirstOrDefault(q => q.Key == "accept");

			if (!String.IsNullOrEmpty(accept.Value))
			{
				request.Headers.Accept.Clear();
				request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept.Value));
			}

			return base.SendAsync(request, cancellationToken);
		}
	}
}