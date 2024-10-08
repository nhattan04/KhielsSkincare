using KhielsSkincare.Controllers;
using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Mvc;

namespace KhielsSkincare.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly KhielsContext _khielsContext;
        public ProductController(KhielsContext khielsContext)
        {
            _khielsContext = khielsContext;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
