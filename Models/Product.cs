namespace KhielsSkincare.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public string SkinType { get; set; } // Ví dụ: "Da nhờn", "Da khô"
        public string SkinCondition { get; set; } // Ví dụ: "Mụn", "Lão hóa"

        public virtual Category Category { get; set; }
        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
        public virtual ProductDetail ProductDetail { get; set; }
        public virtual ICollection<Discount> Discounts { get; set; } // Liên kết với Discount
    }

}
