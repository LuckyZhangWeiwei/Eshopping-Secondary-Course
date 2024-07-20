using AutoMapper;
using Ordering.Application.Responses;
using Ording.Core.Entities;

namespace Ordering.Application.Mappers
{
    public class OrderMapperProfile : Profile
    {
        public OrderMapperProfile()
        {
            CreateMap<Order, OrderResponse>().ReverseMap();
        }
    }
}
