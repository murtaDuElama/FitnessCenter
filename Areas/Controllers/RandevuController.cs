using FitnessCenter.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RandevuController : Controller
    {
        private readonly AppDbContext _context;

        public RandevuController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = _context.Randevular
                .Include(x => x.Hizmet)
                .Include(x => x.Antrenor)
                .ToList();

            return View(model);
        }
    }
}
