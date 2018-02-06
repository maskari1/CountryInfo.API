using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CountryInfo.API.Services
{
    using System.Web.Http.ExceptionHandling;

    using log4net;

    // Install-Package log4net

    public class Log4NetExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            if (context.ExceptionContext.ControllerContext != null)
            {
                var log = LogManager.GetLogger(context.ExceptionContext.ControllerContext.Controller.GetType());
                log.ErrorFormat(
                    "Unhandled exception processing {0} for {1}: {2}",
                    context.Request.Method,
                    context.Request.RequestUri,
                    context.Exception);
            }
        }
    }
}