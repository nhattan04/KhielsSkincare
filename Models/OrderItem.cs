namespace KhielsSkincare.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }

        public virtual Order Order { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
    }
}