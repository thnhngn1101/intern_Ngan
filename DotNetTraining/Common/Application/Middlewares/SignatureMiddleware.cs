using Common.Application.Exceptions;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;

namespace Common.Application.Middlewares
{
    public class SignatureMiddleware : BaseMiddleware
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SignatureMiddleware> _logger;

        public SignatureMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<SignatureMiddleware> logger) : base(next)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private static string ComputeMD5(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();


        }

        public override async Task Invoke(HttpContext context, IServiceProvider services, IConfiguration configuration)
        {
            try
            {
                await ValidateSignatureAsync(context, configuration);
                // Skip signature validation for the static file
                var trimmedUrlFirst = context.Request.GetEncodedPathAndQuery().StartsWith("/api") ? context.Request.GetEncodedPathAndQuery().Substring(4) : context.Request.GetEncodedPathAndQuery();
                if (trimmedUrlFirst.Contains("StaticFiles", StringComparison.OrdinalIgnoreCase))
                {
                    await next(context);
                }
                else
                {
                    // Skip signature validation for the Development environment
                    if (string.IsNullOrEmpty(_configuration["ASPNETCORE_ENVIRONMENT"]) ||
                        !_configuration["ASPNETCORE_ENVIRONMENT"].Equals("Development", StringComparison.OrdinalIgnoreCase))
                    {
                        await ValidateSignatureAsync(context, configuration);
                    }

                    await next(context);
                }


            }
            catch (Exception ex)
            {
               _logger.LogError(ex, "Error in SignatureMiddleware");
                await HandleForbiddenResponseAsync(context);
            }
        }

        private async Task ValidateSignatureAsync(HttpContext context, IConfiguration configuration)
        {

            var signature = context.Request.Headers["Signature"].ToString();
            var time = context.Request.Headers["Time"].ToString();

            if (string.IsNullOrWhiteSpace(signature) || string.IsNullOrWhiteSpace(time))
            {
                await HandleForbiddenResponseAsync(context, "Missing signature or time headers.");
                return;
            }

            var body = await GetRequestBodyAsync(context);
            var url = context.Request.GetEncodedPathAndQuery();
            var trimmedUrl = TrimUrl(url);
            var basePath = trimmedUrl.Split('?')[0];
            DateTime currentDate = DateTime.Now;
            string formattedDate = currentDate.ToString("dd-MM-yyyy");
            string data =  basePath + formattedDate + configuration["HashingOptions:SignaturePassword"];
            Console.WriteLine(data);
            var expectedSignature = ComputeMD5(data);
            if (!signature.Equals(expectedSignature, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Headers["BACKEND_Signature"] = expectedSignature;
                if (data.Length < 500)
                {
                    context.Response.Headers["DATA"] = data;
                }
                await HandleForbiddenResponseAsync(context, "Invalid signature.");
            }
        }

        private static bool IsUploadOrNotificationHub(string path)
        {
            return path.Contains("upload", StringComparison.OrdinalIgnoreCase) ||
                   path.Contains("notificationhub", StringComparison.OrdinalIgnoreCase);
        }

        private static async Task<string> GetRequestBodyAsync(HttpContext context)
        {
            if (context.Request.Method.Equals(HttpMethods.Get, StringComparison.OrdinalIgnoreCase))
                return null;

            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            return body;
        }

        private static async Task HandleForbiddenResponseAsync(HttpContext context, string message = "Forbidden")
        {
            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await context.Response.WriteAsync(message);
        }
        public string TrimUrl(string url)
        {
            var trimmedUrl = url.StartsWith("/api/api")
                ? url.Substring(9)
                : url.StartsWith("/api")
                    ? url.Substring(4)
                    : url;

            if (!trimmedUrl.StartsWith("/"))
            {
                trimmedUrl = "/" + trimmedUrl;
            }

            return trimmedUrl;
        }

    }
}
