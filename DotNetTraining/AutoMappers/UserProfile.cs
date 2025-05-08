using AutoMapper;
using DotNetTraining.Domains.Dtos;
using DotNetTraining.Domains.Models;
using DotNetTraining.Domains.Entities;

namespace DotNetTraining.AutoMappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDto, User>(); 
            CreateMap<User, UserModel>();
            CreateMap<User, UserDto>(); 

        }
    }
}
