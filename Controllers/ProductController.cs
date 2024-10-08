using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhielsSkincare.Controllers
{
    public class ProductController : Controller
    {
        private readonly KhielsContext _khielsContext;

        public ProductController(ILogger<HomeController> logger, KhielsContext context)
        {            
            _khielsContext = context;
        }
        // GET: ProductController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ProductController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null) return RedirectToAction("Index");

            var product = await _khielsContext.Products
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return RedirectToAction("Index");

            return View(product);
        }

    }
}
