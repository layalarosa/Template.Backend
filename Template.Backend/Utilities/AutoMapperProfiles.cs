using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Template.Backend.Dtos;

namespace Template.Backend.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() 
        {
            ConfigureUserMapping();
        }
        private void ConfigureUserMapping()
        {
            CreateMap<IdentityUser, UserDto>();
        }
    }
}
