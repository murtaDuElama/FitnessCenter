using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Data;
using FitnessCenter.Models;

namespace FitnessCenter.Controllers
{
    public class AntrenorController : Controller
    {
        private readonly AppDbContext _context;

        public AntrenorController(AppDbContext context)
        {
            _context = context;
        }

        // Public antrenör listesi
        public async Task<IActionResult> Index()
        {
            var antrenorler = await _context.Antrenorler.ToListAsync();
            return View(antrenorler);
        }
    }
}
