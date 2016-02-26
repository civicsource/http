using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Archon.WebApi
{
	/// <summary>
	/// If it finds an auth token in the querystring, it moves it to the authorization header.
	/// </summary>
	public class AuthHeaderManipulator : DelegatingHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (request.Headers.Authorization == null)
			{
				var query = request.GetQueryNameValuePairs();
				var auth = query.FirstOrDefault(q => q.Key == "auth");

				if (!String.IsNullOrEmpty(auth.Value))
				{
					request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.Value);
				}
			}

			return base.SendAsync(request, cancellationToken);
		}
	}
}