using AutoMapper;
using InventoryAPI.Models;
using InventoryDAL.Entity;

namespace InventoryAPI.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ItemModel, Item>().ReverseMap();
    }
}