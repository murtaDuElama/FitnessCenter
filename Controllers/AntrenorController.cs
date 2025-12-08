using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Data;
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

    // --------------------- CREATE GET ---------------------
    public IActionResult Create()
    {
        return View();
    }

    // --------------------- CREATE POST ---------------------
    [HttpPost]
    public async Task<IActionResult> Create(Antrenor a)
    {
        if (!ModelState.IsValid)
            return View(a);

        _context.Antrenorler.Add(a);
        await _context.SaveChangesAsync();
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
    public async Task<IActionResult> Edit(Antrenor a)
    {
        if (!ModelState.IsValid)
            return View(a);

        _context.Antrenorler.Update(a);
        await _context.SaveChangesAsync();
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
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ant = await _context.Antrenorler.FindAsync(id);
        if (ant == null)
            return NotFound();

        _context.Antrenorler.Remove(ant);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
