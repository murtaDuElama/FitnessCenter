using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Data;
using FitnessCenter.Models;

namespace FitnessCenter.Controllers // Namespace'in "FitnessCenter.Controllers" olduğundan emin olun
{
    public class HizmetController : Controller
    {
        private readonly AppDbContext _context;

        public HizmetController(AppDbContext context)
        {
            _context = context;
        }

        // Sadece Listeleme Yapar
        public async Task<IActionResult> Index()
        {
            var hizmetler = await _context.Hizmetler.ToListAsync();
            return View(hizmetler);
        }
    }
}