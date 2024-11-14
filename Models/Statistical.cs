namespace KhielsSkincare.Models
{
    public class Statistical
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int Sold { get; set; }
        public int Revenue { get; set; } //doanh thu
        public int Profit { get; set; } //loi nhuan
        public DateTime DateCreate { get; set; }
    }
}
