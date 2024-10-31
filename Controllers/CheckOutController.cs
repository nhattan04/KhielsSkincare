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
                // Chuyển hướng đến trang đăng nhập với ReturnUrl
                return RedirectToAction("Login", "Account", new { returnUrl });
            }

            if (ModelState.IsValid)
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var orderCode = Guid.NewGuid().ToString();
                
                // Tạo mã đơn hàng duy nhất
                List<CartItem> cartItems = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

                // Lưu thông tin giao hàng
                var shipping = new Shipping
                {
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    PhoneNumber = model.PhoneNumber,
                    AddressLine = model.AddressLine,
                    City = model.City,
                    OrderCode = orderCode
                };
                _khielsContext.Shippings.Add(shipping);             

                // Lưu thông tin đơn hàng
                var order = new Order
                {
                    OrderCode = orderCode,
                    UserName = User.Identity.Name,
                    Address = model.AddressLine + ", " + model.City,
                    PhoneNumber = model.PhoneNumber,
                    OrderDate = DateTime.Now,
                    Status = 0, // Pending status
                    TotalAmount = cartItems.Sum(c => c.Price * c.Quantity),
                };
                _khielsContext.Orders.Add(order);

                // Giả sử có giỏ hàng trong session
                

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
                    Amount = cartItems.Sum(c => c.Price * c.Quantity),
                    PaymentDate = DateTime.Now,
                    Status = "Success",
                    OrderCode = orderCode
                };
                _khielsContext.Payments.Add(payment);

                _khielsContext.SaveChanges();
                HttpContext.Session.Remove("Cart");


                // Đọc và chuẩn bị nội dung email HTML
                var templatePath = Path.Combine(_webHostEnvironment.WebRootPath, "emailTemplates", "CheckOutSuccess.html");
                var emailContent = await System.IO.File.ReadAllTextAsync(templatePath);

                // Thay thế các biến trong template
                emailContent = emailContent.Replace("{{UserName}}", User.Identity.Name);
                emailContent = emailContent.Replace("{{OrderCode}}", orderCode);
                emailContent = emailContent.Replace("{{TotalAmount}}", order.TotalAmount.ToVnd());

                // Gửi email
                var receiver = userEmail;
                var subject = "Xác nhận đơn hàng";
                await _emailSender.SendEmailAsync(receiver, subject, emailContent);

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
