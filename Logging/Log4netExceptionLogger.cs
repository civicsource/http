using System;
using System.Web.Http.ExceptionHandling;
using log4net;

namespace Archon.WebApi.Logging
{
	public class Log4netExceptionLogger : ExceptionLogger
	{
		static readonly ILog log = LogManager.GetLogger(typeof(Log4netExceptionLogger));

		public override void Log(ExceptionLoggerContext context)
		{
			string message = String.Format("Unhandled exception processing {0} for {1}.", context.Request.Method, context.Request.RequestUri);
			log.Error(message, context.Exception);
		}
	}
}