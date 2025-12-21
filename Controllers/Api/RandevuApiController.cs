// =============================================================================
// DOSYA: RandevuApiController.cs
// AÇIKLAMA: Randevu REST API endpoint'leri
// NAMESPACE: FitnessCenter.Controllers.Api
// BASE URL: /api/randevular
// YETKİLENDİRME: Bazı endpoint'ler Admin veya giriş yapmış kullanıcı gerektirir
// =============================================================================

using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Controllers.Api
{
    /// <summary>
    /// Randevu REST API controller sınıfı.
    /// Randevu listeleme işlemlerini JSON formatında sunar.
    /// Admin paneli ve kullanıcı arayüzü AJAX çağrıları için kullanılır.
    /// </summary>
    [ApiController]
    [Route("api/randevular")]
    public class RandevuApiController : ControllerBase
    {
        /// <summary>Veritabanı bağlam nesnesi</summary>
        private readonly AppDbContext _context;

        /// <summary>Kullanıcı yönetim servisi</summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// RandevuApiController constructor.
        /// </summary>
        /// <param name="context">Entity Framework DbContext</param>
        /// <param name="userManager">ASP.NET Identity UserManager</param>
        public RandevuApiController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ===================== DTO TANIMLARI =====================

        /// <summary>
        /// Randevu veri transfer nesnesi.
        /// API yanıtlarında kullanılır - ilişkili entity bilgilerini içerir.
        /// </summary>
        /// <param name="Id">Randevu ID</param>
        /// <param name="Tarih">Randevu tarihi</param>
        /// <param name="Saat">Randevu saati</param>
        /// <param name="Onaylandi">Onay durumu</param>
        /// <param name="HizmetId">Hizmet ID</param>
        /// <param name="HizmetAdi">Hizmetin adı</param>
        /// <param name="AntrenorId">Antrenör ID</param>
        /// <param name="AntrenorAdi">Antrenörün adı</param>
        /// <param name="UyeEmail">Üyenin email adresi</param>
        public sealed record RandevuDto(int Id, DateTime Tarih, string Saat, bool Onaylandi,
            int HizmetId, string? HizmetAdi,
            int AntrenorId, string? AntrenorAdi,
            string? UyeEmail);

        // ===================== ENDPOINT'LER =====================

        /// <summary>
        /// Tüm randevuları listeler (sadece Admin).
        /// GET: /api/randevular
        /// Filtreleme parametreleri ile kullanılabilir.
        /// </summary>
        /// <param name="baslangic">Başlangıç tarihi filtresi</param>
        /// <param name="bitis">Bitiş tarihi filtresi</param>
        /// <param name="onaylandi">Onay durumu filtresi</param>
        /// <param name="hizmetId">Hizmet ID filtresi</param>
        /// <param name="antrenorId">Antrenör ID filtresi</param>
        /// <returns>Filtrelenmiş randevu listesi</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]  // Sadece Admin erişebilir
        public async Task<ActionResult<IEnumerable<RandevuDto>>> GetAll(
            [FromQuery] DateTime? baslangic,
            [FromQuery] DateTime? bitis,
            [FromQuery] bool? onaylandi,
            [FromQuery] int? hizmetId,
            [FromQuery] int? antrenorId)
        {
            // Include ile ilişkili verileri getir
            var q = _context.Randevular
                .Include(r => r.Hizmet)
                .Include(r => r.Antrenor)
                .Include(r => r.User)
                .AsQueryable();

            // Tarih aralığı filtreleri
            if (baslangic.HasValue)
                q = q.Where(r => r.Tarih.Date >= baslangic.Value.Date);

            if (bitis.HasValue)
                q = q.Where(r => r.Tarih.Date <= bitis.Value.Date);

            // Onay durumu filtresi
            if (onaylandi.HasValue)
                q = q.Where(r => r.Onaylandi == onaylandi.Value);

            // Hizmet filtresi
            if (hizmetId.HasValue)
                q = q.Where(r => r.HizmetId == hizmetId.Value);

            // Antrenör filtresi
            if (antrenorId.HasValue)
                q = q.Where(r => r.AntrenorId == antrenorId.Value);

            // Sonuçları DTO'ya dönüştür
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

        /// <summary>
        /// Giriş yapmış kullanıcının kendi randevularını getirir.
        /// GET: /api/randevular/benim
        /// Kullanıcı kendi randevularını görüntüleyebilir.
        /// </summary>
        /// <returns>Kullanıcının randevu listesi</returns>
        [HttpGet("benim")]
        [Authorize]  // Giriş yapmış kullanıcı gerekli
        public async Task<ActionResult<IEnumerable<RandevuDto>>> GetMine()
        {
            // Mevcut kullanıcıyı al
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Kullanıcının randevularını getir
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
