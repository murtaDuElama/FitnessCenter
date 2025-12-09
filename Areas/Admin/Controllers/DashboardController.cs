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
    }
}
