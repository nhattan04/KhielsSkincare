namespace KhielsSkincare.Models
{
    public class ShippingFee
    {
        public int Id { get; set; } // ID duy nhất
        public string City { get; set; } // Tên thành phố
        public decimal Fee { get; set; } // Phí vận chuyển
    }
}
