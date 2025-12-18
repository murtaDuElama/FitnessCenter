using FitnessCenter.Data;
using FitnessCenter.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FitnessCenter.Controllers.Api
{
    /// <summary>
    /// Raporlama ve istatistik endpointleri - LINQ sorguları ile gelişmiş filtreleme
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RaporController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RaporController> _logger;

        public RaporController(AppDbContext context, ILogger<RaporController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Tüm randevuları gelişmiş filtreleme ile getirir (LINQ Sorguları)
        /// </summary>
        /// <param name="baslangicTarihi">Başlangıç tarihi</param>
        /// <param name="bitisTarihi">Bitiş tarihi</param>
        /// <param name="hizmetId">Hizmet ID</param>
        /// <param name="antrenorId">Antrenör ID</param>
        /// <param name="sadeceOnaylananlar">Sadece onaylananları getir</param>
        /// <param name="uyeEmail">Üye email adresi</param>
        [HttpGet("randevular")]
        [ProducesResponseType(typeof(List<RandevuRaporDto>), 200)]
        public async Task<IActionResult> GetRandevular(
            [FromQuery] DateTime? baslangicTarihi,
            [FromQuery] DateTime? bitisTarihi,
            [FromQuery] int? hizmetId,
            [FromQuery] int? antrenorId,
            [FromQuery] bool? sadeceOnaylananlar,
            [FromQuery] string? uyeEmail)
        {
            try
            {
                // LINQ ile filtreleme
                var query = _context.Randevular
                    .Include(r => r.Hizmet)
                    .Include(r => r.Antrenor)
                    .Include(r => r.User)
                    .AsQueryable();

                // Tarih aralığı filtresi
                if (baslangicTarihi.HasValue)
                    query = query.Where(r => r.Tarih.Date >= baslangicTarihi.Value.Date);

                if (bitisTarihi.HasValue)
                    query = query.Where(r => r.Tarih.Date <= bitisTarihi.Value.Date);

                // Hizmet filtresi
                if (hizmetId.HasValue)
                    query = query.Where(r => r.HizmetId == hizmetId.Value);

                // Antrenör filtresi
                if (antrenorId.HasValue)
                    query = query.Where(r => r.AntrenorId == antrenorId.Value);

                // Onay durumu filtresi
                if (sadeceOnaylananlar.HasValue)
                    query = query.Where(r => r.Onaylandi == sadeceOnaylananlar.Value);

                // Üye email filtresi
                if (!string.IsNullOrEmpty(uyeEmail))
                    query = query.Where(r => r.User != null && r.User.Email.Contains(uyeEmail));

                // Projection ile DTO'ya dönüştürme
                var result = await query
                    .OrderByDescending(r => r.Tarih)
                    .ThenBy(r => r.Saat)
                    .Select(r => new RandevuRaporDto
                    {
                        Id = r.Id,
                        AdSoyad = r.AdSoyad,
                        UyeEmail = r.User != null ? r.User.Email : "-",
                        Tarih = r.Tarih,
                        Saat = r.Saat,
                        Onaylandi = r.Onaylandi,
                        HizmetAdi = r.Hizmet.Ad,
                        HizmetFiyat = r.Hizmet.Ucret,
                        AntrenorAdi = r.Antrenor.AdSoyad,
                        AntrenorUzmanlik = r.Antrenor.Uzmanlik
                    })
                    .ToListAsync();

                _logger.LogInformation($"Randevu raporu oluşturuldu. {result.Count} kayıt bulundu.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevu raporu oluşturulurken hata oluştu");
                return StatusCode(500, new { error = "Rapor oluşturulurken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Kapsamlı istatistikler - Gelişmiş LINQ Aggregate fonksiyonları
        /// </summary>
        [HttpGet("istatistikler")]
        [ProducesResponseType(typeof(IstatistikRaporDto), 200)]
        public async Task<IActionResult> GetIstatistikler()
        {
            try
            {
                var bugun = DateTime.Today;
                var haftaBaslangic = bugun.AddDays(-(int)bugun.DayOfWeek);
                var ayBaslangic = new DateTime(bugun.Year, bugun.Month, 1);

                // Temel istatistikler - LINQ Count ve Where
                var toplamRandevu = await _context.Randevular.CountAsync();
                var bugunRandevu = await _context.Randevular.CountAsync(r => r.Tarih.Date == bugun);
                var buHaftaRandevu = await _context.Randevular.CountAsync(r => r.Tarih >= haftaBaslangic);
                var buAyRandevu = await _context.Randevular.CountAsync(r => r.Tarih >= ayBaslangic);
                var onayliRandevu = await _context.Randevular.CountAsync(r => r.Onaylandi);
                var bekleyenRandevu = await _context.Randevular.CountAsync(r => !r.Onaylandi);

                // Gelir hesaplamaları - LINQ Sum ve Join
                var toplamGelir = await _context.Randevular
                    .Where(r => r.Onaylandi)
                    .Join(_context.Hizmetler,
                        r => r.HizmetId,
                        h => h.Id,
                        (r, h) => h.Ucret)
                    .SumAsync();

                var buAyGelir = await _context.Randevular
                    .Where(r => r.Onaylandi && r.Tarih >= ayBaslangic)
                    .Join(_context.Hizmetler,
                        r => r.HizmetId,
                        h => h.Id,
                        (r, h) => h.Ucret)
                    .SumAsync();

                // Hizmet bazında istatistikler - LINQ GroupBy ve Aggregate
                var hizmetIstatistikleri = await _context.Randevular
                    .Include(r => r.Hizmet)
                    .GroupBy(r => new { r.HizmetId, r.Hizmet.Ad, r.Hizmet.Ucret })
                    .Select(g => new HizmetIstatistikDto
                    {
                        HizmetAdi = g.Key.Ad,
                        RandevuSayisi = g.Count(),
                        ToplamGelir = g.Count(r => r.Onaylandi) * g.Key.Ucret,
                        OrtalamaFiyat = g.Key.Ucret
                    })
                    .OrderByDescending(h => h.RandevuSayisi)
                    .ToListAsync();

                // Antrenör bazında istatistikler - LINQ GroupBy
                var antrenorIstatistikleri = await _context.Randevular
                    .Include(r => r.Antrenor)
                    .GroupBy(r => new { r.AntrenorId, r.Antrenor.AdSoyad, r.Antrenor.Uzmanlik })
                    .Select(g => new AntrenorIstatistikDto
                    {
                        AntrenorAdi = g.Key.AdSoyad,
                        Uzmanlik = g.Key.Uzmanlik,
                        RandevuSayisi = g.Count(),
                        OnayliRandevuSayisi = g.Count(r => r.Onaylandi),
                        DolulukOrani = g.Count() > 0 ? (decimal)g.Count(r => r.Onaylandi) / g.Count() * 100 : 0
                    })
                    .OrderByDescending(a => a.RandevuSayisi)
                    .ToListAsync();

                // Son 6 ay trend analizi - LINQ GroupBy ve OrderBy
                var altıAyOnce = bugun.AddMonths(-6);
                var aylikTrend = await _context.Randevular
                    .Where(r => r.Tarih >= altıAyOnce)
                    .Include(r => r.Hizmet)
                    .GroupBy(r => new { r.Tarih.Year, r.Tarih.Month })
                    .Select(g => new AylikIstatistikDto
                    {
                        Yil = g.Key.Year,
                        Ay = g.Key.Month,
                        AyAdi = CultureInfo.GetCultureInfo("tr-TR").DateTimeFormat.GetMonthName(g.Key.Month),
                        RandevuSayisi = g.Count(),
                        Gelir = g.Where(r => r.Onaylandi).Sum(r => r.Hizmet.Ucret)
                    })
                    .OrderBy(a => a.Yil)
                    .ThenBy(a => a.Ay)
                    .ToListAsync();

                var rapor = new IstatistikRaporDto
                {
                    ToplamRandevu = toplamRandevu,
                    BugunRandevu = bugunRandevu,
                    BuHaftaRandevu = buHaftaRandevu,
                    BuAyRandevu = buAyRandevu,
                    OnayliRandevu = onayliRandevu,
                    BekleyenRandevu = bekleyenRandevu,
                    ToplamGelir = toplamGelir,
                    BuAyGelir = buAyGelir,
                    HizmetBazindaIstatistikler = hizmetIstatistikleri,
                    AntrenorBazindaIstatistikler = antrenorIstatistikleri,
                    AylikTrend = aylikTrend
                };

                _logger.LogInformation("İstatistik raporu başarıyla oluşturuldu");
                return Ok(rapor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İstatistik raporu oluşturulurken hata oluştu");
                return StatusCode(500, new { error = "İstatistik raporu oluşturulurken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Belirli bir antrenörün detaylı raporu - LINQ ile veri analizi
        /// </summary>
        [HttpGet("antrenor/{antrenorId}")]
        [ProducesResponseType(typeof(AntrenorDetayRaporDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAntrenorDetayRaporu(int antrenorId)
        {
            try
            {
                var antrenor = await _context.Antrenorler.FindAsync(antrenorId);
                if (antrenor == null)
                    return NotFound(new { error = "Antrenör bulunamadı" });

                // Antrenör randevuları - LINQ Where
                var randevular = await _context.Randevular
                    .Include(r => r.Hizmet)
                    .Include(r => r.User)
                    .Where(r => r.AntrenorId == antrenorId)
                    .ToListAsync();

                // İstatistikler
                var toplamRandevu = randevular.Count;
                var onayliRandevu = randevular.Count(r => r.Onaylandi);
                var iptalEdilen = randevular.Count(r => !r.Onaylandi);

                // Benzersiz mesai tarihleri - LINQ Distinct
                var mesaiTarihleri = randevular
                    .Select(r => r.Tarih.Date)
                    .Distinct()
                    .OrderByDescending(d => d)
                    .Take(30)
                    .ToList();

                // En çok tercih edilen saatler - LINQ GroupBy ve OrderBy
                var enCokTercihEdilenSaatler = randevular
                    .GroupBy(r => r.Saat)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => g.Key)
                    .ToList();

                // Son randevular
                var sonRandevular = randevular
                    .OrderByDescending(r => r.Tarih)
                    .Take(10)
                    .Select(r => new RandevuRaporDto
                    {
                        Id = r.Id,
                        AdSoyad = r.AdSoyad,
                        UyeEmail = r.User?.Email ?? "-",
                        Tarih = r.Tarih,
                        Saat = r.Saat,
                        Onaylandi = r.Onaylandi,
                        HizmetAdi = r.Hizmet.Ad,
                        HizmetFiyat = r.Hizmet.Ucret,
                        AntrenorAdi = antrenor.AdSoyad,
                        AntrenorUzmanlik = antrenor.Uzmanlik
                    })
                    .ToList();

                var rapor = new AntrenorDetayRaporDto
                {
                    AntrenorId = antrenorId,
                    AdSoyad = antrenor.AdSoyad,
                    Uzmanlik = antrenor.Uzmanlik,
                    ToplamRandevu = toplamRandevu,
                    OnayliRandevu = onayliRandevu,
                    IptalEdilen = iptalEdilen,
                    MesaiTarihleri = mesaiTarihleri,
                    EnCokTercihEdilenSaatler = enCokTercihEdilenSaatler,
                    SonRandevular = sonRandevular
                };

                return Ok(rapor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Antrenör {antrenorId} için detay raporu oluşturulurken hata");
                return StatusCode(500, new { error = "Rapor oluşturulurken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Gelir raporu - Belirli tarih aralığı için (LINQ Aggregate)
        /// </summary>
        [HttpGet("gelir")]
        [ProducesResponseType(typeof(GelirRaporDto), 200)]
        public async Task<IActionResult> GetGelirRaporu(
            [FromQuery] DateTime? baslangicTarihi,
            [FromQuery] DateTime? bitisTarihi)
        {
            try
            {
                var baslangic = baslangicTarihi ?? DateTime.Today.AddMonths(-1);
                var bitis = bitisTarihi ?? DateTime.Today;

                // Tarih aralığındaki randevular - LINQ Where ve Include
                var randevular = await _context.Randevular
                    .Include(r => r.Hizmet)
                    .Where(r => r.Tarih.Date >= baslangic.Date &&
                                r.Tarih.Date <= bitis.Date &&
                                r.Onaylandi)
                    .ToListAsync();

                var toplamGelir = randevular.Sum(r => r.Hizmet.Ucret);
                var toplamRandevu = randevular.Count;
                var ortalamaSeans = toplamRandevu > 0 ? toplamGelir / toplamRandevu : 0;

                // Günlük detay - LINQ GroupBy
                var gunlukDetay = randevular
                    .GroupBy(r => r.Tarih.Date)
                    .Select(g => new GunlukGelirDto
                    {
                        Tarih = g.Key,
                        RandevuSayisi = g.Count(),
                        Gelir = g.Sum(r => r.Hizmet.Ucret)
                    })
                    .OrderBy(g => g.Tarih)
                    .ToList();

                // Hizmet bazında gelir - LINQ GroupBy ve Sum
                var hizmetBazinda = randevular
                    .GroupBy(r => new { r.HizmetId, r.Hizmet.Ad, r.Hizmet.Ucret })
                    .Select(g => new HizmetGelirDto
                    {
                        HizmetAdi = g.Key.Ad,
                        Miktar = g.Count(),
                        Fiyat = g.Key.Ucret,
                        ToplamGelir = g.Count() * g.Key.Ucret
                    })
                    .OrderByDescending(h => h.ToplamGelir)
                    .ToList();

                var rapor = new GelirRaporDto
                {
                    BaslangicTarihi = baslangic,
                    BitisTarihi = bitis,
                    ToplamGelir = toplamGelir,
                    ToplamRandevu = toplamRandevu,
                    OrtalamaSeans = ortalamaSeans,
                    GunlukDetay = gunlukDetay,
                    HizmetBazinda = hizmetBazinda
                };

                _logger.LogInformation($"Gelir raporu oluşturuldu: {baslangic:d} - {bitis:d}");
                return Ok(rapor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gelir raporu oluşturulurken hata");
                return StatusCode(500, new { error = "Rapor oluşturulurken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Belirli bir tarihte uygun antrenörleri getirir (LINQ ile kompleks sorgu)
        /// </summary>
        [HttpGet("uygun-antrenorler")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetUygunAntrenorler(
            [FromQuery] DateTime tarih,
            [FromQuery] string? saat)
        {
            try
            {
                // O tarihte randevusu olan antrenörler - LINQ Where ve Select
                var doluAntrenorler = await _context.Randevular
                    .Where(r => r.Tarih.Date == tarih.Date &&
                                (string.IsNullOrEmpty(saat) || r.Saat == saat))
                    .Select(r => r.AntrenorId)
                    .Distinct()
                    .ToListAsync();

                // Uygun antrenörler - LINQ Where ve negation
                var uygunAntrenorler = await _context.Antrenorler
                    .Where(a => !doluAntrenorler.Contains(a.Id))
                    .Select(a => new
                    {
                        a.Id,
                        a.AdSoyad,
                        a.Uzmanlik,
                        a.FotografUrl,
                        RandevuSayisi = _context.Randevular.Count(r => r.AntrenorId == a.Id)
                    })
                    .OrderBy(a => a.AdSoyad)
                    .ToListAsync();

                return Ok(new
                {
                    tarih = tarih.ToString("yyyy-MM-dd"),
                    saat = saat ?? "Tüm gün",
                    uygunAntrenorSayisi = uygunAntrenorler.Count,
                    antrenorler = uygunAntrenorler
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Uygun antrenörler sorgulanırken hata");
                return StatusCode(500, new { error = "Sorgu sırasında bir hata oluştu" });
            }
        }

        /// <summary>
        /// Üyenin randevularını getirir (LINQ ile userId bazlı filtreleme)
        /// </summary>
        [HttpGet("uye-randevulari")]
        [ProducesResponseType(typeof(List<RandevuRaporDto>), 200)]
        public async Task<IActionResult> GetUyeRandevulari([FromQuery] string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return BadRequest(new { error = "userId parametresi gerekli" });

                // Üyenin tüm randevuları - LINQ Where, Include ve OrderBy
                var randevular = await _context.Randevular
                    .Include(r => r.Hizmet)
                    .Include(r => r.Antrenor)
                    .Include(r => r.User)
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.Tarih)
                    .ThenByDescending(r => r.Saat)
                    .Select(r => new RandevuRaporDto
                    {
                        Id = r.Id,
                        AdSoyad = r.AdSoyad,
                        UyeEmail = r.User.Email,
                        Tarih = r.Tarih,
                        Saat = r.Saat,
                        Onaylandi = r.Onaylandi,
                        HizmetAdi = r.Hizmet.Ad,
                        HizmetFiyat = r.Hizmet.Ucret,
                        AntrenorAdi = r.Antrenor.AdSoyad,
                        AntrenorUzmanlik = r.Antrenor.Uzmanlik
                    })
                    .ToListAsync();

                return Ok(new
                {
                    userId = userId,
                    toplamRandevu = randevular.Count,
                    onayliRandevu = randevular.Count(r => r.Onaylandi),
                    bekleyenRandevu = randevular.Count(r => !r.Onaylandi),
                    randevular = randevular
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Üye {userId} randevuları sorgulanırken hata");
                return StatusCode(500, new { error = "Sorgu sırasında bir hata oluştu" });
            }
        }
    }
}