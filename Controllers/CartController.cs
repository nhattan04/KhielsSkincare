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
            //TempData["success"] = "Add to Cart Success";
            //return Redirect(Request.Headers["Referer"].ToString());
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Decrease(int variantId)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

            //if (cart == null)
            //{
            //    return Json(new { success = false, message = "Giỏ hàng trống." });
            //}

            CartItem cartItem = cart.FirstOrDefault(c => c.ProductVariantId == variantId);

            //if (cartItem == null)
            //{
            //    return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
            //}

            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
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

            decimal grandTotal = cart.Sum(x => x.Quantity * x.Price); // Tính tổng lại

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

            decimal grandTotal = cart.Sum(x => x.Quantity * x.Price); // Tính tổng lại

            return Json(new { success = true, grandTotal });
        }

        [HttpPost]
        public IActionResult Remove(int variantId)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

            //if (cart == null)
            //{
            //    return Json(new { success = false, message = "Giỏ hàng trống." });
            //}

            CartItem cartItem = cart.FirstOrDefault(c => c.ProductVariantId == variantId);

            if (cartItem != null)
            {
                cart.Remove(cartItem); // Xóa sản phẩm khỏi giỏ hàng
                HttpContext.Session.SetJson("Cart", cart); // Cập nhật lại giỏ hàng trong session
            }

            decimal grandTotal = cart.Sum(x => x.Quantity * x.Price); // Tính tổng lại

            return Json(new { success = true, grandTotal });
        }
    }
}
