using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhielsSkincare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/User")]

    public class UserController : Controller
    {
        private readonly KhielsContext _khielsContext;
        public UserController(KhielsContext khielsContext)
        {
            _khielsContext = khielsContext;
        }
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _khielsContext.Users.OrderByDescending(p => p.Id).ToListAsync());
        }
    }
}
