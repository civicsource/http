using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Archon.WebApi
{
	/// <remarks>
	/// http://stackoverflow.com/a/23097445/316108
	/// </remarks>
	public abstract class MethodConstraintedRouteAttribute : RouteFactoryAttribute
	{
		public HttpMethod Method { get; private set; }

		public override IDictionary<string, object> Constraints
		{
			get
			{
				var constraints = new HttpRouteValueDictionary();
				constraints.Add("method", new MethodConstraint(Method));
				return constraints;
			}
		}

		public MethodConstraintedRouteAttribute(string template, HttpMethod method)
			: base(template)
		{
			Method = method;
		}
	}
}
