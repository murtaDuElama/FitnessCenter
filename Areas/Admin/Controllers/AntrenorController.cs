using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Data;
using FitnessCenter.Models;

namespace FitnessCenter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AntrenorController : Controller
    {
        private readonly AppDbContext _context;

        public AntrenorController(AppDbContext context)
        {
            _context = context;
        }

        // --------------------- LISTE ---------------------
        public async Task<IActionResult> Index()
        {
            var liste = await _context.Antrenorler.ToListAsync();
            return View(liste);
        }

        // --------------------- DETAILS ---------------------
        public async Task<IActionResult> Details(int id)
        {
            var ant = await _context.Antrenorler.FindAsync(id);
            if (ant == null)
                return NotFound();

            return View(ant);
        }

        // --------------------- CREATE GET ---------------------
        public IActionResult Create()
        {
            return View();
        }

        // --------------------- CREATE POST ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Antrenor a)
        {
            if (!ModelState.IsValid)
                return View(a);

            _context.Antrenorler.Add(a);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Antrenör başarıyla eklendi!";
            return RedirectToAction("Index");
        }

        // --------------------- EDIT GET ---------------------
        public async Task<IActionResult> Edit(int id)
        {
            var ant = await _context.Antrenorler.FindAsync(id);
            if (ant == null)
                return NotFound();

            return View(ant);
        }

        // --------------------- EDIT POST ---------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Antrenor a)
        {
            if (!ModelState.IsValid)
                return View(a);

            _context.Antrenorler.Update(a);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Antrenör başarıyla güncellendi!";
            return RedirectToAction("Index");
        }

        // --------------------- DELETE GET ---------------------
        public async Task<IActionResult> Delete(int id)
        {
            var ant = await _context.Antrenorler.FindAsync(id);
            if (ant == null)
                return NotFound();

            return View(ant);
        }

        // --------------------- DELETE POST ---------------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ant = await _context.Antrenorler.FindAsync(id);
            if (ant == null)
                return NotFound();

            _context.Antrenorler.Remove(ant);
            await _context.SaveChangesAsync();
            TempData["Delete"] = "Antrenör başarıyla silindi!";
            return RedirectToAction("Index");
        }
    }
}