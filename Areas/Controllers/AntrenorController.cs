using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Index()
        {
            var liste = _context.Antrenorler.ToList();
            return View(liste);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Antrenor model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Antrenorler.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var ant = _context.Antrenorler.Find(id);
            if (ant == null)
                return NotFound();

            return View(ant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Antrenor model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Antrenorler.Update(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var ant = _context.Antrenorler.Find(id);
            if (ant == null)
                return NotFound();

            return View(ant);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var ant = _context.Antrenorler.Find(id);
            if (ant == null)
                return NotFound();

            _context.Antrenorler.Remove(ant);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var ant = _context.Antrenorler.Find(id);
            if (ant == null)
                return NotFound();

            return View(ant);
        }
    }
}
