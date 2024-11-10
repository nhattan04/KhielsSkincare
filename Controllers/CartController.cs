﻿using KhielsSkincare.Extensions;
using KhielsSkincare.Models;
using KhielsSkincare.Models.ViewModels;
using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KhielsSkincare.Controllers
{
    public class CartController : Controller
    {
        private readonly KhielsContext _khielsContext;
        public CartController(KhielsContext khielsContext)
        {
            _khielsContext = khielsContext;
        }

        public IActionResult Index()
        {
            List<CartItem> cartItems = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Giả sử phí ship là 30,000 VND
            const decimal shippingFee = 30000;
            decimal discountValue = 0; // Giảm giá mặc định là 0

            // Tính tổng tiền giỏ hàng
            decimal provisionalAmount = cartItems.Sum(c => c.Price * c.Quantity);
            decimal grandTotal = provisionalAmount + shippingFee - discountValue;

            // Tạo ViewModel và truyền vào View
            var model = new CheckoutViewModel
            {
                CartItems = cartItems,  // Đảm bảo rằng CartItems được truyền vào ViewModel
                ProvisionalAmount = provisionalAmount,
                ShippingFee = shippingFee,
                DiscountValue = discountValue,
                GrandTotal = grandTotal
            };

            return View(model); // Truyền model vào View
        }

        [HttpPost]
        [Route("Cart/Add")]
        public async Task<IActionResult> Add(int Id, int variantId)
        {
            // Lấy sản phẩm từ cơ sở dữ liệu
            Product product = await _khielsContext.Products.FindAsync(Id);
            ProductVariant variant = await _khielsContext.ProductVariants
                .FirstOrDefaultAsync(v => v.ProductVariantId == variantId && v.ProductId == Id);

            if (product == null || variant == null)
            {
                return NotFound();
            }

            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            CartItem cartItem = cart.FirstOrDefault(c => c.ProductVariantId == variantId);

            if (cartItem == null)
            {
                cart.Add(new CartItem(product, variant));
            }
            else
            {
                cartItem.Quantity += 1;
            }

            HttpContext.Session.SetJson("Cart", cart);
            return Json(new { success = true });
        }


        [HttpPost]
        public async Task<IActionResult> Decrease(int variantId)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");         

            CartItem cartItem = cart.FirstOrDefault(c => c.ProductVariantId == variantId);

            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
            }
            else
            {
                cart.RemoveAll(p => p.ProductVariantId == variantId);
            }

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            decimal grandTotal = cart.Sum(x => x.Total); // Tính lại tổng giỏ hàng
            string formattedTotal = grandTotal.ToVnd(); // Định dạng sang VND

            return Json(new { success = true, grandTotal = formattedTotal });
        }

        [HttpPost]
        public async Task<IActionResult> Increase(int variantId)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

            CartItem cartItem = cart.FirstOrDefault(c => c.ProductVariantId == variantId);

            if (cartItem != null)
            {
                // Lấy thông tin tồn kho của sản phẩm từ cơ sở dữ liệu
                var productVariant = await _khielsContext.ProductVariants.FindAsync(variantId);

                if (productVariant == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                }

                // Kiểm tra tồn kho
                if (cartItem.Quantity < productVariant.Quantity)
                {
                    // Tăng số lượng sản phẩm trong giỏ hàng nếu chưa đạt giới hạn tồn kho
                    cartItem.Quantity++;
                    HttpContext.Session.SetJson("Cart", cart); // Cập nhật giỏ hàng trong session
                }
                else
                {
                    // Trả về thông báo nếu đã đạt giới hạn tồn kho
                    return Json(new { success = false, message = "Không thể thêm sản phẩm vì đã đạt giới hạn tồn kho.", currentQuantity = cartItem.Quantity });
                }
            }

            decimal grandTotal = cart.Sum(x => x.Total); // Tính lại tổng giỏ hàng

            string formattedTotal = grandTotal.ToVnd(); // Định dạng sang VND

            return Json(new { success = true, grandTotal = formattedTotal });
        }

        [HttpPost]
        public IActionResult Remove(int variantId)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");          

            CartItem cartItem = cart.FirstOrDefault(c => c.ProductVariantId == variantId);

            if (cartItem != null) 
            {
                cart.Remove(cartItem); // Xóa sản phẩm khỏi giỏ hàng
                HttpContext.Session.SetJson("Cart", cart); // Cập nhật lại giỏ hàng trong session
            }

            decimal grandTotal = cart.Sum(x => x.Total); // Tính lại tổng giỏ hàng
            if (cart.Count == 0)
            {
                return Json(new { success = true, grandTotal, message = "Giỏ hàng của bạn đã trống." });
            }


            string formattedTotal = grandTotal.ToVnd(); // Định dạng sang VND
            return Json(new { success = true, grandTotal = formattedTotal });
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int variantId, int quantity)
        {
            var cartItems = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Tìm sản phẩm trong giỏ hàng
            var item = cartItems.FirstOrDefault(i => i.ProductVariantId == variantId);

            // Lấy thông tin ProductVariant từ database
            var productVariant = _khielsContext.ProductVariants.FirstOrDefault(v => v.ProductVariantId == variantId);
            if (productVariant == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            // Kiểm tra nếu số lượng yêu cầu vượt quá tồn kho
            if (quantity > productVariant.Quantity)
            {
                return Json(new { success = false, message = $"Số lượng vượt quá tồn kho. Hiện tại chỉ còn {productVariant.Quantity} sản phẩm." });
            }

            if (item != null)
            {
                item.Quantity = quantity; // Cập nhật số lượng nếu số lượng hợp lệ
            }
            else
            {
                // Nếu sản phẩm không tồn tại trong giỏ, thêm vào giỏ hàng với số lượng mới
                cartItems.Add(new CartItem
                {
                    ProductVariantId = variantId,
                    Quantity = quantity,
                    ProductId = productVariant.ProductId,
                    ProductName = productVariant.Product.ProductName,
                    Size = productVariant.Size,
                    Price = productVariant.Price,
                    Image = productVariant.Product.ImageUrl
                });
            }

            // Lưu giỏ hàng vào session
            HttpContext.Session.SetJson("Cart", cartItems);

            return Json(new { success = true, message = "Cập nhật giỏ hàng thành công." });
        }

        [HttpPost]
        public IActionResult CheckDiscountCode(string code, int productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return Json(new { success = false, message = "Mã giảm giá không hợp lệ." });
                }

                // Trim code để đảm bảo không có khoảng trắng thừa
                code = code.Trim();

                // Kiểm tra xem mã giảm giá có tồn tại trong bảng Discount và liên kết với sản phẩm có ProductId cụ thể hay không
                var discount = _khielsContext.Discounts
                    .Where(d => d.Code == code && d.ProductId == productId)
                    .FirstOrDefault();

                if (discount != null && discount.StartDate <= DateTime.Now && discount.EndDate >= DateTime.Now)
                {
                    return Json(new { success = true, discountPercentage = discount.DiscountPercentage });
                }
                else
                {
                    return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn cho sản phẩm này." });
                }
            }
            catch (Exception ex)
            {
                // Gửi thông báo lỗi chi tiết về phía client
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}
