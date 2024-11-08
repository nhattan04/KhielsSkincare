using KhielsSkincare.Areas.Admin.Repository;
using KhielsSkincare.Models;
using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ DbContext vào container
builder.Services.AddDbContext<KhielsContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("ConnectedDB"));
});

builder.Services.AddTransient<IEmailSender, EmailSender>();

// Thêm dịch vụ cho các controller và views
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(30);
    option.Cookie.IsEssential = true;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Cấu hình DbContext và bật EnableSensitiveDataLogging
builder.Services.AddDbContext<KhielsContext>(options =>
    options.UseSqlServer(connectionString)
           .EnableSensitiveDataLogging() // Bật logging dữ liệu nhạy cảm
);

builder.Services.AddRazorPages();

builder.Services.AddIdentity<AppUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<KhielsContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    //options.Lockout.MaxFailedAccessAttempts = 5;
    //options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});
var app = builder.Build();
app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode=0");


// Cấu hình pipeline HTTP request
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Kích hoạt HSTS trong môi trường sản xuất
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
