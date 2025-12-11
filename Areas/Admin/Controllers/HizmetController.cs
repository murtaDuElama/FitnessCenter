using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HizmetController : Controller
    {
        private readonly AppDbContext _context;

        public HizmetController(AppDbContext context)
        {
            _context = context;
        }

        // --------------------- LISTE ---------------------
        public async Task<IActionResult> Index()
        {
            var hizmetler = await _context.Hizmetler.ToListAsync();
            return View(hizmetler);
        }

        // --------------------- DETAILS ---------------------
        public async Task<IActionResult> Details(int id)
        {
            var hizmet = await _context.Hizmetler.FindAsync(id);
            if (hizmet == null)
                return NotFound();

            return View(hizmet);
        }

        // --------------------- CREATE GET ---------------------
        public IActionResult Create()
        {
            return View();
        }

        // --------------------- CREATE POST ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hizmet h)
        {
            if (!ModelState.IsValid)
                return View(h);

            _context.Hizmetler.Add(h);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Hizmet başarıyla eklendi!";
            return RedirectToAction("Index");
        }

        // --------------------- EDIT GET ---------------------
        public async Task<IActionResult> Edit(int id)
        {
            var hizmet = await _context.Hizmetler.FindAsync(id);
            if (hizmet == null)
                return NotFound();

            return View(hizmet);
        }

        // --------------------- EDIT POST ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Hizmet h)
        {
            if (!ModelState.IsValid)
                return View(h);

            _context.Hizmetler.Update(h);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Hizmet başarıyla güncellendi!";
            return RedirectToAction("Index");
        }

        // --------------------- DELETE GET ---------------------
        public async Task<IActionResult> Delete(int id)
        {
            var hizmet = await _context.Hizmetler.FindAsync(id);
            if (hizmet == null)
                return NotFound();

            return View(hizmet);
        }

        // --------------------- DELETE POST ---------------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hizmet = await _context.Hizmetler.FindAsync(id);
            if (hizmet == null)
                return NotFound();

            _context.Hizmetler.Remove(hizmet);
            await _context.SaveChangesAsync();
            TempData["Delete"] = "Hizmet başarıyla silindi!";
            return RedirectToAction("Index");
        }
    }
}