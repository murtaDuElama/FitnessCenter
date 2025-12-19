using FitnessCenter.Data;
using FitnessCenter.Models;
using FitnessCenter.Repositories;
using FitnessCenter.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC + API Controller'lar
builder.Services.AddControllersWithViews();

// Swagger/OpenAPI - REST API Dokümantasyonu
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Fitness Center API",
        Version = "v1",
        Description = "Fitness Center Yönetim Sistemi REST API - LINQ ile Gelişmiş Raporlama",
        Contact = new OpenApiContact
        {
            Name = "Fitness Center",
            Email = "info@fitnesscenter.com"
        }
    });

    // XML dokümantasyon desteği (isteğe bağlı)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// DI (Services + Repositories)
builder.Services.AddScoped<AiService>();
builder.Services.AddScoped<IRandevuService, RandevuService>();

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

    // Swagger UI - Sadece Development ortamında
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fitness Center API V1");
        c.RoutePrefix = "swagger"; // http://localhost:port/swagger
        c.DocumentTitle = "Fitness Center API Documentation";
    });
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

// API Controller endpoint'leri (RaporController gibi)
app.MapControllers();

app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();