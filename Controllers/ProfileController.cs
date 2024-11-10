using KhielsSkincare.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KhielsSkincare.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> ProfileDetail()
        {
            ViewData["CurrentPage"] = "ProfileDetail";
            // Lấy thông tin người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);

            // Truyền thông tin người dùng qua View
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(AppUser model, string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin người dùng
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;

            // Kiểm tra nếu người dùng muốn thay đổi mật khẩu
            if (!string.IsNullOrEmpty(CurrentPassword) && !string.IsNullOrEmpty(NewPassword))
            {
                // Kiểm tra mật khẩu hiện tại
                var passwordCheck = await _userManager.CheckPasswordAsync(user, CurrentPassword);
                if (!passwordCheck)
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu hiện tại không đúng.");
                    return View("ProfileDetail", user); // Trả về trang với thông báo lỗi
                }

                if (NewPassword != ConfirmPassword)
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                    return View("ProfileDetail", user);
                }

                // Đổi mật khẩu
                var result = await _userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi đổi mật khẩu.");
                    return View("ProfileDetail", user);
                }
            }

            // Cập nhật thông tin người dùng
            var updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                TempData["success"] = "Cập nhật thông tin thành công!";
            }
            else
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("ProfileDetail", user);
            }

            return RedirectToAction("ProfileDetail"); // Redirect về trang chi tiết
        }

        public async Task<IActionResult> Orders()
        {
            ViewData["CurrentPage"] = "Orders";
            // Lấy thông tin người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);

            // Truyền thông tin người dùng qua View
            return View(user);
        }
    }
}
