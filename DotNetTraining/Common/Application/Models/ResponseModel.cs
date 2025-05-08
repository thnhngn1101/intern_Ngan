using System.Net;
using Common.Application.Models;
using System.Text.Json.Serialization;
namespace Common.Application.Models
{
    public class ResponseModel
    {
        [JsonPropertyName("httpStatus")]
        public HttpStatusCode HttpStatus { get; set; } = HttpStatusCode.OK;
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; set; } = true;
        [JsonPropertyName("data")]
        public object? Data { get; set; }
        [JsonPropertyName("errorCode")]
        public string ErrorCode {  get; set; }
        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; }

        public static ResponseModel Success<T> (T data, HttpStatusCode statusCode = HttpStatusCode.OK) where T : class
        {
            return new ResponseModel {
                HttpStatus = statusCode,
                IsSuccess = true,
                Data = data, 
            };
        }

        public static ResponseModel Error(String errorMsg, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, string errorCode = "")
        {
            return new ResponseModel
            {
                HttpStatus = statusCode,
                IsSuccess = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMsg,
                Data = null
            };
        }
        public static ResponseModel ErrorWithData<T>(T data, string errorMsg, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, string errorCode = "") where T : class
        {
            return new ResponseModel
            {
                HttpStatus = statusCode,
                IsSuccess = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMsg,
                Data = data
            };
        }
    }
}
