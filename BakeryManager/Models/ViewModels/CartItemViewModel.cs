namespace BakeryManager.Models.ViewModels
{
    public class CartItemViewModel
    {
        public List<CartItemModel> CartItems { get; set; }

        public decimal GrandTotal { get; set; }
        public decimal ShippingPrice { get; set; }
        public string CouponCode { get; internal set; }
    }
}
