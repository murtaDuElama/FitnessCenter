using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Controllers.Api
{
    [ApiController]
    [Route("api/randevular")]
    public class RandevuApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RandevuApiController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public sealed record RandevuDto(int Id, DateTime Tarih, string Saat, bool Onaylandi,
            int HizmetId, string? HizmetAdi,
            int AntrenorId, string? AntrenorAdi,
            string? UyeEmail);

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<RandevuDto>>> GetAll(
            [FromQuery] DateTime? baslangic,
            [FromQuery] DateTime? bitis,
            [FromQuery] bool? onaylandi,
            [FromQuery] int? hizmetId,
            [FromQuery] int? antrenorId)
        {
            var q = _context.Randevular
                .Include(r => r.Hizmet)
                .Include(r => r.Antrenor)
                .Include(r => r.User)
                .AsQueryable();

            if (baslangic.HasValue)
                q = q.Where(r => r.Tarih.Date >= baslangic.Value.Date);

            if (bitis.HasValue)
                q = q.Where(r => r.Tarih.Date <= bitis.Value.Date);

            if (onaylandi.HasValue)
                q = q.Where(r => r.Onaylandi == onaylandi.Value);

            if (hizmetId.HasValue)
                q = q.Where(r => r.HizmetId == hizmetId.Value);

            if (antrenorId.HasValue)
                q = q.Where(r => r.AntrenorId == antrenorId.Value);

            var list = await q
                .OrderByDescending(r => r.Tarih)
                .ThenByDescending(r => r.Saat)
                .Select(r => new RandevuDto(
                    r.Id,
                    r.Tarih,
                    r.Saat,
                    r.Onaylandi,
                    r.HizmetId,
                    r.Hizmet != null ? r.Hizmet.Ad : null,
                    r.AntrenorId,
                    r.Antrenor != null ? r.Antrenor.AdSoyad : null,
                    r.User != null ? r.User.Email : null
                ))
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("benim")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RandevuDto>>> GetMine()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var list = await _context.Randevular
                .Include(r => r.Hizmet)
                .Include(r => r.Antrenor)
                .Where(r => r.UserId == user.Id)
                .OrderByDescending(r => r.Tarih)
                .ThenByDescending(r => r.Saat)
                .Select(r => new RandevuDto(
                    r.Id,
                    r.Tarih,
                    r.Saat,
                    r.Onaylandi,
                    r.HizmetId,
                    r.Hizmet != null ? r.Hizmet.Ad : null,
                    r.AntrenorId,
                    r.Antrenor != null ? r.Antrenor.AdSoyad : null,
                    user.Email
                ))
                .ToListAsync();

            return Ok(list);
        }
    }
}
