using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Settings;
using AutoMapper;
using BPMaster.Services;
using Common.Controllers;
using DotNetTraining.Domains.Dtos;
using DotNetTraining.Domains.Entities;
using DotNetTraining.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DotNetTraining.Controllers.v1
{
    [Route("api/users")]
    [ApiController]
    [Authorize] //
    public class UsersController : BaseV1Controller<UserService, ApplicationSetting>
    {
        private readonly UserService _userService;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public UsersController(IServiceProvider services, IHttpContextAccessor httpContextAccessor,
                               IConfiguration config, IMapper mapper)
            : base(services, httpContextAccessor)
        {
            _userService = services.GetService<UserService>()!;
            _config = config;
            _mapper = mapper;
        }

       
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var user = await _userService.GetAllUsers();
            return Success(user);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            return Success(await _userService.GetUserById(userId));
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] UserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return CreatedSuccess(await _service.CreateUser(dto));
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserDto userdto)
        {
            return Success(await _userService.UpdateUser(userId, userdto));
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            await _userService.DeleteUser(userId);
            return Success("Delete success");
        }
    }
}
