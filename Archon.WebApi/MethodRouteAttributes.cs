using System.Net.Http;

namespace Archon.WebApi
{
	//http://stackoverflow.com/a/23097445/316108

	public class GetRouteAttribute : MethodConstraintedRouteAttribute
	{
		public GetRouteAttribute(string template) : base(template, HttpMethod.Get) { }
	}

	public class HeadRouteAttribute : MethodConstraintedRouteAttribute
	{
		public HeadRouteAttribute(string template) : base(template, HttpMethod.Head) { }
	}

	public class PostRouteAttribute : MethodConstraintedRouteAttribute
	{
		public PostRouteAttribute(string template) : base(template, HttpMethod.Post) { }
	}

	public class PatchRouteAttribute : MethodConstraintedRouteAttribute
	{
		public PatchRouteAttribute(string template) : base(template, new HttpMethod("PATCH")) { }
	}

	public class PutRouteAttribute : MethodConstraintedRouteAttribute
	{
		public PutRouteAttribute(string template) : base(template, HttpMethod.Put) { }
	}

	public class DeleteRouteAttribute : MethodConstraintedRouteAttribute
	{
		public DeleteRouteAttribute(string template) : base(template, HttpMethod.Delete) { }
	}
}
