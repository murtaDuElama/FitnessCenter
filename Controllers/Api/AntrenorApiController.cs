// =============================================================================
// DOSYA: AntrenorApiController.cs
// AÇIKLAMA: Antrenör REST API endpoint'leri
// NAMESPACE: FitnessCenter.Controllers.Api
// BASE URL: /api/antrenorler
// SWAGGER: Swagger UI'da görüntülenebilir (/swagger)
// =============================================================================

using FitnessCenter.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Controllers.Api
{
    /// <summary>
    /// Antrenör REST API controller sınıfı.
    /// Antrenör listesi ve müsaitlik bilgilerini JSON formatında sunar.
    /// Frontend AJAX çağrıları ve entegrasyon için kullanılır.
    /// </summary>
    [ApiController]
    [Route("api/antrenorler")]
    public class AntrenorApiController : ControllerBase
    {
        /// <summary>
        /// Veritabanı bağlam nesnesi
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// AntrenorApiController constructor.
        /// </summary>
        /// <param name="context">Entity Framework DbContext</param>
        public AntrenorApiController(AppDbContext context)
        {
            _context = context;
        }

        // ===================== DTO TANIMLARI =====================

        /// <summary>
        /// Antrenör veri transfer nesnesi.
        /// API yanıtlarında kullanılır - sadece gerekli alanları içerir.
        /// </summary>
        /// <param name="Id">Antrenör ID</param>
        /// <param name="AdSoyad">Antrenörün adı soyadı</param>
        /// <param name="Uzmanlik">Uzmanlık alanı</param>
        /// <param name="FotografUrl">Profil fotoğrafı URL'i</param>
        public sealed record AntrenorDto(int Id, string AdSoyad, string Uzmanlik, string? FotografUrl);

        // ===================== ENDPOINT'LER =====================

        /// <summary>
        /// Tüm antrenörleri listeler.
        /// GET: /api/antrenorler
        /// GET: /api/antrenorler?uzmanlik=Fitness
        /// GET: /api/antrenorler?hizmetId=1
        /// </summary>
        /// <param name="uzmanlik">Uzmanlık alanı filtresi (opsiyonel)</param>
        /// <param name="hizmetId">Hizmet ID'sine göre filtre - hizmet adı uzmanlık olarak kullanılır</param>
        /// <returns>Filtrelenmiş antrenör listesi</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AntrenorDto>>> GetAll([FromQuery] string? uzmanlik, [FromQuery] int? hizmetId)
        {
            // Başlangıç sorgusu
            var q = _context.Antrenorler.AsQueryable();

            // HizmetId verilmişse, hizmet adını uzmanlık olarak kullan
            if (hizmetId.HasValue)
            {
                var hizmet = await _context.Hizmetler.FindAsync(hizmetId.Value);
                if (hizmet == null)
                    return NotFound(new { message = "Hizmet bulunamadı." });

                uzmanlik = hizmet.Ad;
            }

            // Uzmanlık filtresi uygula
            if (!string.IsNullOrWhiteSpace(uzmanlik))
            {
                q = q.Where(a => a.Uzmanlik == uzmanlik);
            }

            // Sonuçları DTO'ya dönüştür ve döndür
            var list = await q
                .OrderBy(a => a.AdSoyad)
                .Select(a => new AntrenorDto(a.Id, a.AdSoyad, a.Uzmanlik, a.FotografUrl))
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Belirli tarih ve saatte müsait antrenörleri getirir.
        /// GET: /api/antrenorler/uygun?tarih=2024-01-15&saat=10:00
        /// Randevu oluşturma akışında AJAX ile kullanılır.
        /// </summary>
        /// <param name="tarih">Kontrol edilecek tarih</param>
        /// <param name="saat">Kontrol edilecek saat (HH:mm)</param>
        /// <param name="hizmetId">Hizmet ID filtresi (opsiyonel)</param>
        /// <param name="uzmanlik">Uzmanlık filtresi (opsiyonel)</param>
        /// <returns>Müsait antrenör listesi</returns>
        [HttpGet("uygun")]
        public async Task<ActionResult<IEnumerable<AntrenorDto>>> GetAvailable(
            [FromQuery] DateTime tarih,
            [FromQuery] string? saat,
            [FromQuery] int? hizmetId,
            [FromQuery] string? uzmanlik)
        {
            // Tarih kontrolü - varsayılan bugün
            var targetDate = (tarih == default ? DateTime.Today : tarih.Date);

            // HizmetId'den uzmanlık belirle
            if (hizmetId.HasValue)
            {
                var hizmet = await _context.Hizmetler.FindAsync(hizmetId.Value);
                if (hizmet == null)
                    return NotFound(new { message = "Hizmet bulunamadı." });

                uzmanlik = hizmet.Ad;
            }

            var q = _context.Antrenorler.AsQueryable();

            // Uzmanlık filtresi
            if (!string.IsNullOrWhiteSpace(uzmanlik))
            {
                q = q.Where(a => a.Uzmanlik == uzmanlik);
            }

            // O tarihte dolu randevuları sorgula
            var doluIdsQuery = _context.Randevular
                .Where(r => r.Tarih.Date == targetDate);

            // Saat filtresi varsa
            if (!string.IsNullOrWhiteSpace(saat))
            {
                // Öğle arası - hiç müsait antrenör yok
                if (saat == "12:00")
                    return Ok(new List<AntrenorDto>());

                // Çalışma saatleri içinde mi kontrol et
                q = q.Where(a => string.Compare(a.CalismaBaslangicSaati, saat) <= 0
                             && string.Compare(a.CalismaBitisSaati, saat) >= 0);

                // Sadece o saatteki randevuları filtrele
                doluIdsQuery = doluIdsQuery.Where(r => r.Saat == saat);
            }

            // Dolu antrenör ID'lerini al
            var doluAntrenorIds = await doluIdsQuery
                .Select(r => r.AntrenorId)
                .Distinct()
                .ToListAsync();

            // Müsait antrenörleri getir (dolu olmayanlar)
            var list = await q
                .Where(a => !doluAntrenorIds.Contains(a.Id))
                .OrderBy(a => a.AdSoyad)
                .Select(a => new AntrenorDto(a.Id, a.AdSoyad, a.Uzmanlik, a.FotografUrl))
                .ToListAsync();

            return Ok(list);
        }
    }
}
