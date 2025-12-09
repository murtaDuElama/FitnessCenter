using FitnessCenter.Data;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Identity;
    
var builder = WebApplication.CreateBuilder(args);

// MVC Servisleri
builder.Services.AddControllersWithViews();

// 1. Veritabanı Bağlantıları
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 2. Identity Ayarları (Şifre Kurallarını Esnetme)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // "sau" şifresi için güvenlik kurallarını kapatıyoruz
    options.Password.RequireDigit = false;           // Rakam zorunluluğu yok
    options.Password.RequireLowercase = false;       // Küçük harf zorunluluğu yok
    options.Password.RequireUppercase = false;       // Büyük harf zorunluluğu yok
    options.Password.RequireNonAlphanumeric = false; // Özel karakter (!,+,*) zorunluluğu yok
    options.Password.RequiredLength = 3;             // En az 3 karakter (sau için)
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

var app = builder.Build();

// 3. Veritabanı ve Admin Kullanıcısı Oluşturma (Seed Data)
// Bu kısım SeedData.cs dosyanızdaki "Seed(IApplicationBuilder app)" metodunu çağırır.
await SeedData.Seed(app);

// 4. Hata Yönetimi ve Pipeline
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

// 5. Yetkilendirme (Sırası Önemli: Önce AuthN, Sonra AuthZ)
app.UseAuthentication();
app.UseAuthorization();

// 6. Rotalar
// Admin Paneli Rotası
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

// Varsayılan Rota
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();