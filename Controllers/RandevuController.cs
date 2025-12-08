using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Data;
using FitnessCenter.Models;

public class RandevuController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public RandevuController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ----------------------------------------------------
    // TÜM RANDEVULAR (Admin için)
    // ----------------------------------------------------
    public async Task<IActionResult> Index()
    {
        var randevular = await _context.Randevular
            .Include(r => r.Hizmet)
            .Include(r => r.Antrenor)
            .ToListAsync();

        return View(randevular);
    }

    // ----------------------------------------------------
    // RANDEVU ALMA SAYFASI (GET)
    // ----------------------------------------------------
    public IActionResult Create()
    {
        ViewBag.Hizmetler = _context.Hizmetler.ToList();
        ViewBag.Antrenorler = _context.Antrenorler.ToList();
        return View();
    }

    // ----------------------------------------------------
    // RANDEVU ALMA (POST)
    // ----------------------------------------------------
    [HttpPost]
    public async Task<IActionResult> Create(Randevu r)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Hizmetler = _context.Hizmetler.ToList();
            ViewBag.Antrenorler = _context.Antrenorler.ToList();
            return View(r);
        }

        // ⭐ Giriş yapan kullanıcının ID'sini al
        var userId = _userManager.GetUserId(User);
        r.UserId = userId;   // → Randevuya kullanıcıyı bağladık

        _context.Randevular.Add(r);
        await _context.SaveChangesAsync();

        // Kullanıcıyı kendi randevuları sayfasına yönlendir
        return RedirectToAction("MyRandevus");
    }

    // ----------------------------------------------------
    // GİRİŞ YAPAN KULLANICININ RANDEVULARI
    // ----------------------------------------------------
    public async Task<IActionResult> MyRandevus()
    {
        var userId = _userManager.GetUserId(User);

        var randevular = await _context.Randevular
            .Include(r => r.Hizmet)
            .Include(r => r.Antrenor)
            .Where(r => r.UserId == userId)
            .ToListAsync();

        return View(randevular);
    }

}
