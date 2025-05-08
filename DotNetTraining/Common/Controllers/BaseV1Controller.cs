using System.Net;
using Asp.Versioning;
using AutoMapper;
using Common.Application.Exceptions;
using Common.Application.Models;
using Common.Application.Settings;
using Common.Loggers.Interfaces;
using Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class BaseV1Controller<T, S> : ControllerBase where S: BaseAppSetting where T : class
    {
        protected readonly T _service; //services.GetRequiredService<T>();
        protected ILogManager _logger; //services.GetRequiredService<ILogManager>();
        protected IMapper _mapper; //services.GetRequiredService<IMapper>();
        protected readonly IHttpContextAccessor _httpContextAccessor;
        public AuthenticatedUserModel? _currentUser;//services.GetRequiredService<AuthUserService<S>>().GetUser();
        public BaseV1Controller(IServiceProvider services, IHttpContextAccessor httpContextAccessor)
        {
            _service = services.GetRequiredService<T>();
            _logger = services.GetRequiredService<ILogManager>();
            //_mapper = services.GetRequiredService<IMapper>();
            _httpContextAccessor = httpContextAccessor;

            // Retrieve the current user from HttpContext
            _currentUser = _httpContextAccessor.HttpContext?.Items["User"] as AuthenticatedUserModel;
        }
       

        protected IActionResult Success(object result)
        {
            return Ok(ResponseModel.Success(result));
        }

        protected IActionResult Error(string message)
        {
            return Ok(ResponseModel.Error(message));
        }

        protected IActionResult CreatedSuccess(object result)
        {
            return Created(Request.Path, ResponseModel.Success(result, HttpStatusCode.Created));
        }

        protected IActionResult ErrorWithData(object result, string errorMsg)
        {
            return Ok(ResponseModel.ErrorWithData(result, errorMsg));
        }


    }
}
