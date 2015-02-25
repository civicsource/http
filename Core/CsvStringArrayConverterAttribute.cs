using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;

namespace Archon.WebApi
{
	public class CsvStringArrayConverterAttribute : ActionFilterAttribute
	{
		readonly string name;

		public CsvStringArrayConverterAttribute(string name)
		{
			this.name = name;
		}

		public override void OnActionExecuting(HttpActionContext context)
		{
			string csv = ExtractCommaSeparatedValue(context);

			if (!String.IsNullOrWhiteSpace(csv))
			{
				context.ActionArguments[name] = csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
			}
			else
			{
				context.ActionArguments[name] = new string[0];
			}
		}

		string ExtractCommaSeparatedValue(HttpActionContext context)
		{
			if (!context.ActionArguments.ContainsKey(name))
				return null;

			if (context.ControllerContext.RouteData.Values.ContainsKey(this.name))
				return (string)context.ControllerContext.RouteData.Values[this.name];

			var qs = context.ControllerContext.Request.RequestUri.ParseQueryString();
			return qs[name];
		}
	}
}
