using FitnessCenter.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Controllers.Api
{
    [ApiController]
    [Route("api/raporlar")]
    public class RaporController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RaporController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("randevular")]
        public async Task<IActionResult> GetRandevular(DateTime? tarih, int? hizmetId, int? antrenorId, bool? onayli)
        {
            var query = _context.Randevular
                .Include(r => r.Hizmet)
                .Include(r => r.Antrenor)
                .Include(r => r.User)
                .AsQueryable();

            if (tarih.HasValue)
                query = query.Where(r => r.Tarih.Date == tarih.Value.Date);

            if (hizmetId.HasValue)
                query = query.Where(r => r.HizmetId == hizmetId.Value);

            if (antrenorId.HasValue)
                query = query.Where(r => r.AntrenorId == antrenorId.Value);

            if (onayli.HasValue)
                query = query.Where(r => r.Onaylandi == onayli.Value);

            var result = await query
                .OrderByDescending(r => r.Tarih)
                .Select(r => new
                {
                    r.Id,
                    r.AdSoyad,
                    r.Saat,
                    r.Onaylandi,
                    Tarih = r.Tarih.ToString("yyyy-MM-dd"),
                    Hizmet = r.Hizmet.Ad,
                    Antrenor = r.Antrenor.AdSoyad,
                    Uye = r.User != null ? r.User.Email : "-"
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("istatistikler")]
        public async Task<IActionResult> GetIstatistikler()
        {
            var bugun = DateTime.Today;

            var toplam = await _context.Randevular.CountAsync();
            var bugunRandevu = await _context.Randevular.CountAsync(r => r.Tarih.Date == bugun);

            var hizmetBazli = await _context.Randevular
                .Include(r => r.Hizmet)
                .GroupBy(r => r.Hizmet.Ad)
                .Select(g => new { Hizmet = g.Key, Adet = g.Count() })
                .OrderByDescending(x => x.Adet)
                .ToListAsync();

            return Ok(new
            {
                Toplam = toplam,
                Bugun = bugunRandevu,
                HizmetBazli = hizmetBazli
            });
        }
    }
}
