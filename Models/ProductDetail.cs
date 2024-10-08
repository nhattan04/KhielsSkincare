namespace KhielsSkincare.Models
{
    public class ProductDetail
    {
        public int ProductDetailId { get; set; }
        public int ProductId { get; set; }
        public string Introduction { get; set; } // Giới thiệu sản phẩm
        public string Benefits { get; set; }      // Công dụng sản phẩm
        public string Ingredients { get; set; }   // Thành phần sản phẩm
        public string UsageInstructions { get; set; } // Cách sử dụng

        public string SkinTypeName { get; set; }
        public string SkinConditionName { get; set; }
        public virtual Product Product { get; set; }
    }
}
