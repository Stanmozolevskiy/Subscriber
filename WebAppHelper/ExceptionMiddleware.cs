using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WebAppHelper
{
	/// <summary>
	/// The ExceptionMiddleware class is used to catch exceptions that originate from controller entry points.
	/// This eliminates the need to have exception handlers at every entry point and instead just rely on a single
	/// point in the pipeline to catch, report, and respond to exceptions.
	/// </summary>
	/// <remarks>
	/// This is a middleware scoped to the lifetime of each client request (to allow using the HttpRequest instance associated with the request).
	/// As such, DI via constructor is not approprate: it will force the middleware to behave like a singleton.
	/// Instead DI is done at the InvokeAsync method level. This adds a twist: the middleware can't implement the IMiddleware interface,
	/// since that interface forces the signatgure of the InvokeAsync method to NOT accept addition parameters (which we need for DI).
	/// So, we just implement the middleware WITHOUT deriving from that interface.
	/// </remarks>
	public class ExceptionMiddleware
	{
		public ExceptionMiddleware(RequestDelegate nextDelegate)
		{
			this.nextDelegate = nextDelegate;
		}

		public async Task InvokeAsync(HttpContext httpContext, ILogger<ExceptionMiddleware> logger)
		{
			try
			{
				await nextDelegate(httpContext);
			}
			catch (Exception ex)
			{
				await handleException(httpContext, ex, logger);
			}
		}

		/// <summary>
		/// Methods are expected to throw exceptions as-necessary, and the preferred type of exception to throw is
		/// the "StatusCodeException" which is an extension on the base Exception type with the addition of an action
		/// specific code to propagate.
		/// When an entity is being updated, there may a scenario where the entity in memory (managed by EF) is out-of-sync
		/// with the persisted state of that entity. When this is detected, EF thows a "DbUpdateConcurrencyException" instance.
		/// The aforementioned StatusCodeException and DbUpdateConcurrencyException are specical cases and dealt with as such.
		/// </summary>
		/// <param name="context">Current HTTP context</param>
		/// <param name="exception">Exception to handle</param>
		/// <param name="logger">Scribe to log errors to</param>
		/// <returns>Task</returns>
		private Task handleException(HttpContext context, Exception exception, ILogger<ExceptionMiddleware> logger)
		{
			if ((context.Response.StatusCode == default(int)) || (context.Response.StatusCode == StatusCodes.Status200OK))
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;

			XElement error = new XElement("Error",
				new XAttribute("statusCode", context.Response.StatusCode),
				new XElement("Request", context.GetRequestURL()),
				new XElement("Message", string.Join(Environment.NewLine, exception.Flatten().Select(x => x.Message))),
				new XElement("StackTrace", exception.StackTrace));

			string errorMessage = error.ToString();
			logger.LogError(errorMessage);
			return context.Response.WriteAsync(errorMessage);
		}

		private readonly RequestDelegate nextDelegate;
	}

	internal static class ExceptionExtensions
	{
		internal static IEnumerable<Exception> Flatten(this Exception ex)
		{
			do { yield return ex; } while ((ex = ex.InnerException) != null);
		}
	}
}
