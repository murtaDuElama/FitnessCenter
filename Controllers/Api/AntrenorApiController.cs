using System;
using System.Linq;
using System.Threading.Tasks;
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

        // GET: /api/antrenorler?uzmanlik=...
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? uzmanlik)
        {
            var query = _context.Antrenorler.AsQueryable();

            if (!string.IsNullOrWhiteSpace(uzmanlik))
            {
                query = query.Where(a => a.Uzmanlik.Contains(uzmanlik));
            }

            var antrenorler = await query
                .OrderBy(a => a.AdSoyad)
                .Select(a => new
                {
                    a.Id,
                    a.AdSoyad,
                    a.Uzmanlik,
                    a.FotografUrl
                })
                .ToListAsync();

            return Ok(antrenorler);
        }

        // GET: /api/antrenorler/uygun?tarih=2025-12-15&saat=10:00
        [HttpGet("uygun")]
        public async Task<IActionResult> GetAvailable([FromQuery] DateTime tarih, [FromQuery] string? saat)
        {
            var bookedQuery = _context.Randevular
                .Where(r => r.Tarih.Date == tarih.Date);

            if (!string.IsNullOrWhiteSpace(saat))
            {
                bookedQuery = bookedQuery.Where(r => r.Saat == saat);
            }

            var bookedIds = await bookedQuery
                .Select(r => r.AntrenorId)
                .Distinct()
                .ToListAsync();

            var available = await _context.Antrenorler
                .Where(a => !bookedIds.Contains(a.Id))
                .OrderBy(a => a.AdSoyad)
                .Select(a => new
                {
                    a.Id,
                    a.AdSoyad,
                    a.Uzmanlik
                })
                .ToListAsync();

            return Ok(available);
        }
    }
}