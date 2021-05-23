using System.Threading.Tasks;

namespace Basket.API.GrpcServices
{
    public class DiscountGrpcService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient _protoServiceClient;

        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient protoServiceClient)
        {
            _protoServiceClient = protoServiceClient;
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            var discountRequest = new GetDiscountRequest {ProductName = productName};
            return await _protoServiceClient.GetDiscountAsync(discountRequest);
        }
    }
}