using KhielsSkincare.Areas.Admin.Repository;
using KhielsSkincare.Models;
using KhielsSkincare.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;

namespace KhielsSkincare.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment; 

        public AccountController(SignInManager<AppUser> signInManger, UserManager<AppUser> userManager, IEmailSender sender, IWebHostEnvironment webHostEnvironment)
        {
            _signInManager = signInManger;
            _userManager = userManager;
            _emailSender = sender;
            _webHostEnvironment = webHostEnvironment; 
        }
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl=returnUrl});
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginViewModel.UserName, loginViewModel.Password, false, false);
                if (result.Succeeded)
                {
                    return Redirect(loginViewModel.ReturnUrl ?? "/");
                }
                ModelState.AddModelError("", "Invalid User or Pass");
            }
            return View(loginViewModel);
        }
        public IActionResult Register()
        {
            return View();
        }      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            //if (ModelState.IsValid)
            //{
            //    AppUser newUser = new AppUser { UserName = user.UserName, Email = user.Email };
            //    IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);

            //    if (result.Succeeded)
            //    {
            //        // Tạo mã xác nhận
            //        var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            //        var userId = await _userManager.GetUserIdAsync(newUser);

            //        // Tạo ConfirmationLink
            //        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = userId, code = code }, Request.Scheme);

            //        // Đọc nội dung file HTML
            //        var emailTemplatePath = Path.Combine(_webHostEnvironment.WebRootPath, "emailTemplates", "EmailTemplate.html");
            //        var emailTemplate = await System.IO.File.ReadAllTextAsync(emailTemplatePath);

            //        // Thay thế các biến trong template
            //        emailTemplate = emailTemplate.Replace("{{UserName}}", user.UserName);
            //        emailTemplate = emailTemplate.Replace("{{ConfirmationLink}}", confirmationLink); // Thay thế link xác nhận

            //        // Gửi email xác thực
            //        await _emailSender.SendEmailAsync(newUser.Email, "Xác thực tài khoản", emailTemplate);

            //        TempData["success"] = "Tạo tài khoản thành công. Hãy kiểm tra email để xác nhận.";
            //        return Redirect("/home/index");
            //    }

            //    foreach (IdentityError error in result.Errors)
            //    {
            //        ModelState.AddModelError("", error.Description);
            //    }
            //}
            //return View();

            if (ModelState.IsValid)
            {
                AppUser newUser = new AppUser { UserName = user.UserName, Email = user.Email };
                IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);
                if (result.Succeeded)
                {

                    TempData["success"] = "Tạo tài khoản thành công. Hãy kiểm tra email để xác nhận.";
                    return Redirect("/home/index");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View("ConfirmEmail"); // Tạo view để thông báo xác thực thành công
            }
            return View("Error"); // Tạo view để thông báo lỗi
        }
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(returnUrl);
        }
    }
}
