using FitnessCenter.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Controllers.Api
{
    [ApiController]
    [Route("api/antrenorler")]
    public class AntrenorApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AntrenorApiController(AppDbContext context)
        {
            _context = context;
        }

        public sealed record AntrenorDto(int Id, string AdSoyad, string Uzmanlik, string? FotografUrl);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AntrenorDto>>> GetAll([FromQuery] string? uzmanlik, [FromQuery] int? hizmetId)
        {
            var q = _context.Antrenorler.AsQueryable();

            if (hizmetId.HasValue)
            {
                var hizmet = await _context.Hizmetler.FindAsync(hizmetId.Value);
                if (hizmet == null)
                    return NotFound(new { message = "Hizmet bulunamadı." });

                uzmanlik = hizmet.Ad;
            }

            if (!string.IsNullOrWhiteSpace(uzmanlik))
            {
                q = q.Where(a => a.Uzmanlik == uzmanlik);
            }

            var list = await q
                .OrderBy(a => a.AdSoyad)
                .Select(a => new AntrenorDto(a.Id, a.AdSoyad, a.Uzmanlik, a.FotografUrl))
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("uygun")]
        public async Task<ActionResult<IEnumerable<AntrenorDto>>> GetAvailable(
            [FromQuery] DateTime tarih,
            [FromQuery] string? saat,
            [FromQuery] int? hizmetId,
            [FromQuery] string? uzmanlik)
        {
            var targetDate = (tarih == default ? DateTime.Today : tarih.Date);

            if (hizmetId.HasValue)
            {
                var hizmet = await _context.Hizmetler.FindAsync(hizmetId.Value);
                if (hizmet == null)
                    return NotFound(new { message = "Hizmet bulunamadı." });

                uzmanlik = hizmet.Ad;
            }

            var q = _context.Antrenorler.AsQueryable();

            if (!string.IsNullOrWhiteSpace(uzmanlik))
            {
                q = q.Where(a => a.Uzmanlik == uzmanlik);
            }

            var doluIdsQuery = _context.Randevular
                .Where(r => r.Tarih.Date == targetDate);

            if (!string.IsNullOrWhiteSpace(saat))
            {
                if (saat == "12:00")
                    return Ok(new List<AntrenorDto>());

                q = q.Where(a => string.Compare(a.CalismaBaslangicSaati, saat) <= 0
                             && string.Compare(a.CalismaBitisSaati, saat) >= 0);

                doluIdsQuery = doluIdsQuery.Where(r => r.Saat == saat);
            }

            var doluAntrenorIds = await doluIdsQuery
                .Select(r => r.AntrenorId)
                .Distinct()
                .ToListAsync();

            var list = await q
                .Where(a => !doluAntrenorIds.Contains(a.Id))
                .OrderBy(a => a.AdSoyad)
                .Select(a => new AntrenorDto(a.Id, a.AdSoyad, a.Uzmanlik, a.FotografUrl))
                .ToListAsync();

            return Ok(list);
        }
    }
}
