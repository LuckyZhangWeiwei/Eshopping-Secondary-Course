using AutoMapper;
using EventBus.Messages.Events;
using Ordering.Application.Commands;
using Ordering.Application.Responses;
using Ording.Core.Entities;

namespace Ordering.Application.Mappers;

public class OrderMapperProfile : Profile
{
    public OrderMapperProfile()
    {
        CreateMap<Order, OrderResponse>().ReverseMap();

        CreateMap<Order, CheckoutOrderCommand>().ReverseMap();

        CreateMap<Order, UpdateOrderCommand>().ReverseMap();

        CreateMap<CheckoutOrderCommand, BasketCheckoutEvent>().ReverseMap();

        //CreateMap<CheckoutOrderCommand, BasketCheckoutEventV2>().ReverseMap();
    }
}
