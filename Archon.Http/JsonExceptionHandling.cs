using System;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

namespace Archon.Http
{
	public static class JsonExceptionHandling
	{
		public static IApplicationBuilder UseJsonExceptionHandling(IApplicationBuilder app)
		{
			app.UseExceptionHandler(builder =>
			{
				builder.Run(async context =>
				{
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					context.Response.ContentType = "application/json";
					var ex = context.Features.Get<IExceptionHandlerFeature>();
					if (ex != null)
					{
						var err = JsonConvert.SerializeObject(BuildError(ex.Error));
						await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(err), 0, err.Length).ConfigureAwait(false);
					}

					object BuildError(Exception exception)
					{
						if (exception == null)
							return null;

						return new
						{
							exception.Message,
							exception.StackTrace,
							InnerException = BuildError(exception.InnerException)
						};
					}
				});
			});

			return app;
		}
	}
}
