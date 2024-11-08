using KhielsSkincare.Areas.Admin.Repository;
using KhielsSkincare.Extensions;
using KhielsSkincare.Models;
using KhielsSkincare.Models.ViewModels;
using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KhielsSkincare.Controllers
{
    [Authorize]
    public class CheckOutController : Controller
    {
        private readonly KhielsContext _khielsContext;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CheckOutController(KhielsContext khielsContext, IEmailSender sender, IWebHostEnvironment webHostEnvironment)
        {
            _khielsContext = khielsContext;
            _emailSender = sender;
            _webHostEnvironment = webHostEnvironment;
        }
        // Action xác nhận thanh toán
        [HttpPost]
        public async Task<IActionResult> ProcessCheckout(CheckoutViewModel model, string returnUrl = null)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl });
            }

            if (ModelState.IsValid)
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var orderCode = Guid.NewGuid().ToString();

                List<CartItem> cartItems = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

                // Giả sử phí ship cố định là 30,000đ
                const decimal shippingFee = 30000;

                // Lấy giá trị mã giảm giá từ model, nếu có
                decimal discountValue = model.DiscountValue ?? 0;

                // Tính tổng tiền giỏ hàng, cộng phí ship và trừ giảm giá
                decimal provisionalAmount = cartItems.Sum(c => c.Price * c.Quantity);
                decimal totalAmount = provisionalAmount + shippingFee - discountValue;

                var order = new Order
                {
                    OrderCode = orderCode,
                    UserName = User.Identity.Name,
                    Address = model.AddressLine + ", " + model.City,
                    PhoneNumber = model.PhoneNumber,
                    OrderDate = DateTime.Now,
                    Status = 0, // Trạng thái chờ xử lý
                    TotalAmount = totalAmount // Lưu tổng tiền đã tính phí ship và giảm giá
                };
                _khielsContext.Orders.Add(order);

                // Lưu chi tiết đơn hàng
                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderCode = orderCode,
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Size = item.Size
                    };
                    _khielsContext.OrderDetails.Add(orderDetail);
                }

                // Lưu thông tin thanh toán
                var payment = new Payment
                {
                    PaymentMethod = model.PaymentMethod,
                    Amount = totalAmount,
                    PaymentDate = DateTime.Now,
                    Status = "Success",
                    OrderCode = orderCode
                };
                _khielsContext.Payments.Add(payment);

                // Lưu vào cơ sở dữ liệu
                _khielsContext.SaveChanges();
                HttpContext.Session.Remove("Cart");

                // Đọc và chuẩn bị nội dung email HTML
                var templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "emailTemplates", "CheckOutSuccess.html");
                var emailContent = await System.IO.File.ReadAllTextAsync(templatePath);

                // Thay thế các biến trong template email
                emailContent = emailContent.Replace("{{UserName}}", User.Identity.Name);
                emailContent = emailContent.Replace("{{OrderCode}}", orderCode);
                emailContent = emailContent.Replace("{{TotalAmount}}", totalAmount.ToVnd());

                // Gửi email
                await _emailSender.SendEmailAsync(userEmail, "Xác nhận đơn hàng", emailContent);

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        public IActionResult ThankYou()
        {
            return View();
        }

    }
}
