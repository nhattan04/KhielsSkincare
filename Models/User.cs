namespace KhielsSkincare.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Password { get; set; } // Mật khẩu
        public string Role { get; set; }     // Vai trò (ví dụ: Admin, Customer)

        public virtual ICollection<Order> Orders { get; set; }
    }

}
