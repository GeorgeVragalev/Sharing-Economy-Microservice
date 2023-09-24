using AutoMapper;
using OrderAPI.Models;
using OrderDAL.Entity;

namespace OrderAPI.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<OrderModel, Order>().ReverseMap();
        CreateMap<PlaceOrderRequestModel, Order>().ReverseMap();
    }
}