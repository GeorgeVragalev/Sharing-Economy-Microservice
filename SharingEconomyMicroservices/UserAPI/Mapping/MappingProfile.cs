using AutoMapper;
using DAL.Entity;
using UserAPI.Models;

namespace UserAPI.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserModel, User>().ReverseMap();
    }
}