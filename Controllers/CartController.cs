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
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Total)

            };
            return View(cartVM);
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

            return Json(new { success = true, grandTotal });
        }

        [HttpPost]
        public async Task<IActionResult> Increase(int variantId)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

            CartItem cartItem = cart.FirstOrDefault(c => c.ProductVariantId == variantId);

            if (cartItem != null)
            {
                ++cartItem.Quantity;
                HttpContext.Session.SetJson("Cart", cart);
            }

            decimal grandTotal = cart.Sum(x => x.Total); // Tính lại tổng giỏ hàng

            return Json(new { success = true, grandTotal });
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


            return Json(new { success = true, grandTotal });
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int variantId, int quantity)
        {
            var cartItems = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Tìm sản phẩm trong giỏ hàng
            var item = cartItems.FirstOrDefault(i => i.ProductVariantId == variantId);
            if (item != null)
            {
                item.Quantity = quantity; // Cập nhật số lượng
            }
            else
            {
                // Nếu sản phẩm không tồn tại trong giỏ, bạn có thể thêm vào (tuỳ thuộc vào logic của bạn)
                cartItems.Add(new CartItem { ProductVariantId = variantId, Quantity = quantity });
            }

            // Lưu giỏ hàng vào session
            HttpContext.Session.SetJson("Cart", cartItems);

            return Json(new { success = true, message = "Cập nhật giỏ hàng thành công." });
        }

    }
}
