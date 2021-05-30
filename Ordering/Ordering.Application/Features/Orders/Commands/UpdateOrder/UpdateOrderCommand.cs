using MediatR;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommand: IRequest
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public decimal TotalPrice { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string EmailAddress { get; set; }
        public string AddressLine { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string CardName { get; set; }
        public string CardHolder { get; set; }
        public string Expiration { get; set; }
        public string Cvc { get; set; }
        public string PaymentMethod { get; set; }  
    }
}