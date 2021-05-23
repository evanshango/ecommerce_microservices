namespace Discount.API.Entities
#pragma warning disable 8618

{
    public class Coupon 
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
    }
}