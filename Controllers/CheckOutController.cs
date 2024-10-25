using KhielsSkincare.Areas.Admin.Repository;
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
        public CheckOutController(KhielsContext khielsContext, IEmailSender sender)
        {
            _khielsContext = khielsContext;
            _emailSender = sender;
        }
        public async Task<IActionResult> CheckOut(string returnUrl = null)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Chuyển hướng đến trang đăng nhập với ReturnUrl
                return RedirectToAction("Login", "Account", new { returnUrl });
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var orderCode = Guid.NewGuid().ToString();
            var orderItem = new Order
            {
                OrderCode = orderCode,
                UserName = userEmail,
                Status = 1,
                OrderDate = DateTime.Now
            };

            _khielsContext.Orders.Add(orderItem);
            _khielsContext.SaveChanges();

            List<CartItem> cartItems = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            foreach (var cart in cartItems)
            {
                var orderDetails = new OrderDetail
                {
                    UserName = userEmail,
                    OrderCode = orderCode,
                    ProductId = cart.ProductId,
                    ProductName = cart.ProductName,
                    Price = cart.Price,
                    Size = cart.Size,
                    Quantity = cart.Quantity
                };

                _khielsContext.OrderDetails.Add(orderDetails);
            }

            _khielsContext.SaveChanges();
            HttpContext.Session.Remove("Cart");
            var receiver = userEmail;
            var subject = "Đơn hàng của bạn";
            var message = "Đơn hàng của bạn đã đặt thành công";
            await _emailSender.SendEmailAsync(receiver, subject, message);
            TempData["success"] = "Check out success";
            return RedirectToAction("Index", "Cart");
        }
    }

}
