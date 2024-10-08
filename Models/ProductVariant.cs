namespace KhielsSkincare.Models
{
    public class ProductVariant
    {
        public int ProductVariantId { get; set; }
        public int ProductId { get; set; }
        public string Size { get; set; } // Dung tích
        public decimal Price { get; set; } // Giá tiền

        public virtual Product Product { get; set; }
    }

}
