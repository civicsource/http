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
		readonly Type type;

		public CsvStringArrayConverterAttribute(string name, Type type)
		{
			this.name = name;
			this.type = type;
		}

		public override void OnActionExecuting(HttpActionContext context)
		{
			string csv = ExtractCommaSeparatedValue(context);

			if (!String.IsNullOrWhiteSpace(csv))
			{
				context.ActionArguments[name] = csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => ConvertString(s.Trim())).ToArray();
			}
			else
			{
				context.ActionArguments[name] = new object[0];
			}
		}

		object ConvertString(string str)
		{
			if (type == typeof(Guid))
			{
				return new Guid(str);
			}
			else
			{
				return Convert.ChangeType(str, this.type);
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
