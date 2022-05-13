using CustomerService.Models;
using LoggerService;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace CustomerService.ExceptionMiddleware
{
	public class CustomExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILoggerManager _logger;

		public CustomExceptionMiddleware(RequestDelegate next, ILoggerManager logger)
		{
			_logger = logger;
			_next = next;
		}

		public async Task InvokeAsync(HttpContext httpContext,ILoggerManager manager)
		{
			try
			{
				manager.LogError("test");
				await _next(httpContext);
			}
			catch (Exception error)
			{
				var response = httpContext.Response;
				response.ContentType = "application/json";

				switch (error)
				{
					case AppException e:
						// custom application error
						_logger.LogError($"A exception has been thrown: {e}");
						response.StatusCode = (int)HttpStatusCode.BadRequest;
						break;
					default:
						// unhandled error
						_logger.LogError($"Something went wrong");
						response.StatusCode = (int)HttpStatusCode.InternalServerError;
						break;
				}

				var result = JsonSerializer.Serialize(new { message = error?.Message });
				await response.WriteAsync(result);
			}
			
		}

		
	}
}
