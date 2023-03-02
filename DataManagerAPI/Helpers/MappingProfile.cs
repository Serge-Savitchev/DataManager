using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;

namespace DataManagerAPI.Helpers;

/// <summary>
/// Mapping data.
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Mapping profile.
    /// </summary>
    public MappingProfile()
    {
        //CreateMap<UserDto?, User>()
        //    .ForMember(user => user.UserCredentials, c => new UserCredentials())
        //    .ForMember(user => user.Role, c => c.MapFrom(t => Enum.Parse<RoleId>(t!.Role, true)));
        //    .ReverseMap();

        CreateMap<RegisteredUserDto?, User>()
            .ForMember(user => user.Role, c => c.MapFrom(t => Enum.Parse<RoleIds>(t!.Role, true)));

        CreateMap<UserDto, User>()
            .ForMember(user => user.Role, c => c.MapFrom(t => Enum.Parse<RoleIds>(t!.Role, true)))
        .ReverseMap()
            .ForMember(user => user.Role, c => c.MapFrom(t => t.Role.ToString()));

        CreateMap<User, UserDetailsDto>()
            .ForMember(user => user.Role, c => c.MapFrom(t => t.Role.ToString()));

        CreateMap<AddUserDataDto, UserData>();

        CreateMap<UserDataDto, UserData>()
            .ReverseMap();

        CreateMap<User, LoginUserResponseDto>()
            .ForMember(user => user.Role, c => c.MapFrom(t => t.Role.ToString()));

        CreateMap<UserFileDto, UserFile>()
            .ReverseMap();

        CreateMap<UserFileStreamDto, UserFileStream>()
            .ReverseMap();
    }
}
