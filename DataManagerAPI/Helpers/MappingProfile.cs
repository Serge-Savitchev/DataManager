﻿using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Models;

namespace DataManagerAPI.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //CreateMap<UserDto?, User>()
        //    .ForMember(user => user.UserCredentials, c => new UserCredentials())
        //    .ForMember(user => user.Role, c => c.MapFrom(t => Enum.Parse<RoleId>(t!.Role, true)));
        //    .ReverseMap();

        CreateMap<AddUserDto?, User>()
            .ForMember(user => user.UserCredentials, c => c.MapFrom(t => CredentialsHelper.CreatePasswordHash(t!.Password)))
            .ForMember(user => user.Role, c => c.MapFrom(t => Enum.Parse<RoleId>(t!.Role, true)));

        CreateMap<UserDto, User>()
            .ForMember(user => user.Role, c => c.MapFrom(t => Enum.Parse<RoleId>(t!.Role, true)))
        .ReverseMap()
            .ForMember(user => user.Role, c => c.MapFrom(t => Enum.GetName(typeof(RoleId), t.Role)));

        CreateMap<AddUserDataDto, UserData>();

        CreateMap<UserDataDto, UserData>()
            .ReverseMap();
        //CreateMap<UserData, UserDataDto>();
    }
}
