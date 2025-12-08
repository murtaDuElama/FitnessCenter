using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Data;

public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard()
    {
        var model = new AdminDashboardViewModel
        {
            HizmetSayisi = await _context.Hizmetler.CountAsync(),
            AntrenorSayisi = await _context.Antrenorler.CountAsync(),
            RandevuSayisi = await _context.Randevular.CountAsync(),

            SonHizmetler = await _context.Hizmetler
                                .OrderByDescending(x => x.Id)
                                .Take(5).ToListAsync(),

            SonAntrenorler = await _context.Antrenorler
                                  .OrderByDescending(x => x.Id)
                                  .Take(5).ToListAsync(),

            SonRandevular = await _context.Randevular
                                .Include(x => x.Hizmet)
                                .Include(x => x.Antrenor)
                                .OrderByDescending(x => x.Id)
                                .Take(5).ToListAsync()
        };


        return View(model);
    }
    public async Task<IActionResult> HizmetList()
    {
        var list = await _context.Hizmetler.ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> AntrenorList()
    {
        var list = await _context.Antrenorler.ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> RandevuList()
    {
        var list = await _context.Randevular
            .Include(r => r.Hizmet)
            .Include(r => r.Antrenor)
            .ToListAsync();

        return View(list);
    }

}
