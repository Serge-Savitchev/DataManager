using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Models;
using System.Security.Cryptography;

namespace DataManagerAPI.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<UserDto?, User>()
            //    .ForMember(user => user.UserCredentials, c => new UserCredentials())
            //    .ForMember(user => user.Role, c => c.MapFrom(t => Enum.Parse<RoleId>(t!.Role, true)));
            //    .ReverseMap();

            CreateMap<AddUserDto?, User>()
                .ForMember(user => user.UserCredentials, c => c.MapFrom(t => CreatePasswordHash(t)))
                .ForMember(user => user.Role, c => c.MapFrom(t => Enum.Parse<RoleId>(t!.Role, true)));

            CreateMap<User, UserDto>()
                .ForMember(user => user.Role, c => c.MapFrom(t => Enum.GetName(typeof(RoleId), t.Role)));

            CreateMap<UserDataDto, UserData>()
                .ReverseMap();
            //CreateMap<UserData, UserDataDto>();
        }

        private static UserCredentials CreatePasswordHash(AddUserDto? data)
        {
            var result = new UserCredentials();
            if (data is not null)
            {
                using (var hmac = new HMACSHA512())
                {
                    result.PasswordSalt = hmac.Key;
                    result.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data.Password));
                }
            }
            return result;
        }
    }
}
