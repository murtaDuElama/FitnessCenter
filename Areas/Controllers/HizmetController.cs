using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Index()
        {
            var hizmetler = _context.Hizmetler.ToList();
            return View(hizmetler);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Hizmet model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Hizmetler.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var hizmet = _context.Hizmetler.Find(id);
            if (hizmet == null)
                return NotFound();

            return View(hizmet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Hizmet model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Hizmetler.Update(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var hizmet = _context.Hizmetler.Find(id);
            if (hizmet == null)
                return NotFound();

            return View(hizmet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var hizmet = _context.Hizmetler.Find(id);
            if (hizmet == null)
                return NotFound();

            _context.Hizmetler.Remove(hizmet);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
