﻿using Common.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ApplicationException = Common.Application.Exceptions.ApplicationException;

namespace Common.Application.Middlewares
{
	public class GlobalExceptionHandlerMiddleware(RequestDelegate next) : BaseMiddleware(next)
	{
        public override async Task Invoke(HttpContext context, IServiceProvider services, IConfiguration configuration)
		{
			try
			{
                await next(context);
            }
			catch (Exception ex)
			{
				var appException = GetApplicationException(ex);
				context.Response.ContentType = "application/json";
				context.Response.StatusCode = (int)appException.HttpStatusCode;

                var response = appException.GetErrorResponse();
                var json = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                await context.Response.WriteAsync(json);
            }
		}
		private static ApplicationException GetApplicationException(Exception ex)
		{
			return ex.GetType().IsSubclassOf(typeof(ApplicationException))
				? (ApplicationException)ex : new InternalErrorException(ex.Message);
		}

	}
}
