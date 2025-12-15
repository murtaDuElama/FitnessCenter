using FitnessCenter.Data;
using FitnessCenter.Models;
using FitnessCenter.Repositories;
using FitnessCenter.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC + API Controller’lar
builder.Services.AddControllersWithViews();

// DI (Services + Repositories)
builder.Services.AddScoped<AiService>();
builder.Services.AddScoped<RandevuService>();

builder.Services.AddScoped<IHizmetRepository, HizmetRepository>();
builder.Services.AddScoped<IAntrenorRepository, AntrenorRepository>();
builder.Services.AddScoped<IRandevuRepository, RandevuRepository>();

// 1) AppDbContext (Identity + Uygulama modelleri)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 2) Identity Ayarları
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

var app = builder.Build();

// 3) Seed Data
await SeedData.Seed(app);

// 4) Hata Yönetimi
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 5) Yetkilendirme
app.UseAuthentication();
app.UseAuthorization();

// API Controller endpoint’leri (RaporController gibi)
app.MapControllers();

app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
