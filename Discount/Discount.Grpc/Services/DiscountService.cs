using System.Threading.Tasks;
using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepo _repo;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepo repo, ILogger<DiscountService> logger, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repo.GetDiscount(request.ProductName);
            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound,
                    $"Discount with ProductName={request.ProductName} cannot be found."));
            }

            _logger.LogInformation(
                $"Discount is being retrieved fro ProductName : {coupon.ProductName}, Amount: {coupon.Amount}");

            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            await _repo.CreateDiscount(coupon);

            _logger.LogInformation($"Discount has been created. ProductName : {coupon.ProductName}");
            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            await _repo.UpdateDiscount(coupon);

            _logger.LogInformation($"Discount has been updated. ProductName : {coupon.ProductName}");
            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request,
            ServerCallContext context)
        {
            var deleted = await _repo.DeleteDiscount(request.ProductName);
            var response = new DeleteDiscountResponse {Success = deleted};
            return response;
        }
    }
}