using AutoMapper;
using Discount.Core.Entites;
using Discount.Grpc.Protos;

namespace Discount.Application.Mapper;

public class DiscountProfile : Profile
{
    public DiscountProfile()
    {
        CreateMap<Coupon, CouponModel>().ReverseMap();
    }
}
