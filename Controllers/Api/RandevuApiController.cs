using System;
using System.Linq;
using System.Threading.Tasks;
using FitnessCenter.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Controllers.Api
{
    [ApiController]
    [Route("api/randevular")]
    public class RandevuApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RandevuApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/randevular?baslangic=2025-12-01&bitis=2025-12-31&userId=...&antrenorId=1
        [HttpGet]
        public async Task<IActionResult> GetFiltered(
            [FromQuery] DateTime? baslangic,
            [FromQuery] DateTime? bitis,
            [FromQuery] string? userId,
            [FromQuery] int? antrenorId)
        {
            var query = _context.Randevular
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .AsQueryable();

            if (baslangic.HasValue)
            {
                query = query.Where(r => r.Tarih.Date >= baslangic.Value.Date);
            }

            if (bitis.HasValue)
            {
                query = query.Where(r => r.Tarih.Date <= bitis.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(userId))
            {
                query = query.Where(r => r.UserId == userId);
            }

            if (antrenorId.HasValue)
            {
                query = query.Where(r => r.AntrenorId == antrenorId.Value);
            }

            var randevular = await query
                .OrderBy(r => r.Tarih)
                .ThenBy(r => r.Saat)
                .Select(r => new
                {
                    r.Id,
                    r.AdSoyad,
                    r.UserId,
                    r.Tarih,
                    r.Saat,
                    r.Onaylandi,
                    Hizmet = new { r.HizmetId, r.Hizmet.Ad },
                    Antrenor = new { r.AntrenorId, r.Antrenor.AdSoyad }
                })
                .ToListAsync();

            return Ok(randevular);
        }

        // GET: /api/randevular/gunluk?gun=2025-12-15&sadeceOnayli=true
        [HttpGet("gunluk")]
        public async Task<IActionResult> GetByDay([FromQuery] DateTime gun, [FromQuery] bool sadeceOnayli = false)
        {
            var dailyQuery = _context.Randevular
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .Where(r => r.Tarih.Date == gun.Date);

            if (sadeceOnayli)
            {
                dailyQuery = dailyQuery.Where(r => r.Onaylandi);
            }

            var gunlukListe = await dailyQuery
                .OrderBy(r => r.Saat)
                .Select(r => new
                {
                    r.Id,
                    r.AdSoyad,
                    r.Saat,
                    Hizmet = r.Hizmet.Ad,
                    Antrenor = r.Antrenor.AdSoyad,
                    r.Onaylandi
                })
                .ToListAsync();

            return Ok(gunlukListe);
        }
    }
}
