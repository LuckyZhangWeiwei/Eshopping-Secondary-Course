using AutoMapper;
using Discount.Application.Queries;
using Discount.Core.Repositories;
using Discount.Grpc.Protos;
using Grpc.Core;
using MediatR;

namespace Discount.Application.Handlers;

public class GetDiscountQueryHandler : IRequestHandler<GetDiscountQuery, CouponModel>
{
    private readonly IDiscountRepository _discountRespoitory;
    private readonly IMapper _mapper;

    public GetDiscountQueryHandler(IDiscountRepository discountRepository, IMapper mapper)
    {
        _discountRespoitory = discountRepository;
        _mapper = mapper;
    }

    public async Task<CouponModel> Handle(
        GetDiscountQuery request,
        CancellationToken cancellationToken
    )
    {
        var coupon = await _discountRespoitory.GetDiscount(request.ProductName);

        if (coupon == null)
        {
            throw new RpcException(
                new Status(
                    StatusCode.NotFound,
                    $"Discount with product name = {request.ProductName} not found"
                )
            );
        }

        return _mapper.Map<CouponModel>(coupon);
    }
}
