using FitnessCenter.Data;
using FitnessCenter.Areas.Admin.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _appDb;
        private readonly AuthDbContext _authDb;

        public DashboardController(AppDbContext appDb, AuthDbContext authDb)
        {
            _appDb = appDb;
            _authDb = authDb;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalHizmet = _appDb.Hizmetler.Count(),
                TotalAntrenor = _appDb.Antrenorler.Count(),
                TotalRandevu = _appDb.Randevular.Count(),
                TotalUsers = _authDb.Users.Count()
            };

            return View(model);
        }

        // TÜM RANDEVULAR
        public IActionResult Randevular()
        {
            var randevular = _appDb.Randevular
                .Include(r => r.Hizmet)
                .Include(r => r.Antrenor)
                .Include(r => r.User)
                .OrderByDescending(r => r.Tarih)
                .ToList();

            return View(randevular);
        }

    }
}
