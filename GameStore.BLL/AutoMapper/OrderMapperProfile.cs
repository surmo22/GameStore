using AutoMapper;
using GameStore.BLL.DTOs.Orders;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.Relations;

namespace GameStore.BLL.AutoMapper;

public class OrderMapperProfile : Profile
{
    public OrderMapperProfile()
    {
        CreateMap<Order, OrderDto>()
            .ReverseMap();
        CreateMap<OrderGame, OrderGameDto>()
            .ReverseMap();
    }
}