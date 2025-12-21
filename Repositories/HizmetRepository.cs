// =============================================================================
// DOSYA: HizmetRepository.cs
// AÇIKLAMA: Hizmet repository implementasyonu - Entity Framework ile veri erişimi
// NAMESPACE: FitnessCenter.Repositories
// PATTERN: Repository Pattern - IHizmetRepository'nin EF Core implementasyonu
// =============================================================================

using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Repositories
{
    /// <summary>
    /// Hizmet repository implementasyonu.
    /// IHizmetRepository interface'ini Entity Framework Core ile uygular.
    /// AppDbContext üzerinden Hizmetler tablosuna erişim sağlar.
    /// </summary>
    public class HizmetRepository : IHizmetRepository
    {
        /// <summary>
        /// Entity Framework DbContext - veritabanı bağlantısı
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// HizmetRepository constructor.
        /// </summary>
        /// <param name="context">AppDbContext (DI ile alınır)</param>
        public HizmetRepository(AppDbContext context)
        {
            _context = context;
        }

        // ===================== OKUMA İŞLEMLERİ =====================

        /// <summary>
        /// Tüm hizmetleri veritabanından getirir.
        /// </summary>
        /// <returns>Hizmet listesi</returns>
        public Task<List<Hizmet>> GetAllAsync()
        {
            return _context.Hizmetler.ToListAsync();
        }

        /// <summary>
        /// ID'ye göre hizmet getirir.
        /// </summary>
        /// <param name="id">Hizmet ID</param>
        /// <returns>Hizmet veya null</returns>
        public Task<Hizmet?> GetByIdAsync(int id)
        {
            return _context.Hizmetler.FirstOrDefaultAsync(h => h.Id == id);
        }

        // ===================== YAZMA İŞLEMLERİ =====================

        /// <summary>
        /// Yeni hizmet ekler ve değişiklikleri kaydeder.
        /// Hizmet adı unique olmalıdır (veritabanı constraint).
        /// </summary>
        /// <param name="hizmet">Eklenecek hizmet</param>
        public async Task AddAsync(Hizmet hizmet)
        {
            _context.Hizmetler.Add(hizmet);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Hizmet bilgilerini günceller.
        /// EF Core tracking sorunlarını önlemek için mevcut entity detach edilir.
        /// </summary>
        /// <param name="hizmet">Güncellenecek hizmet</param>
        public async Task UpdateAsync(Hizmet hizmet)
        {
            // Tracking sorununu önle: aynı ID'li tracked entity varsa detach et
            var existingEntry = _context.ChangeTracker.Entries<Hizmet>()
                .FirstOrDefault(e => e.Entity.Id == hizmet.Id);

            if (existingEntry != null)
            {
                _context.Entry(existingEntry.Entity).State = EntityState.Detached;
            }

            _context.Hizmetler.Update(hizmet);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Hizmeti veritabanından siler.
        /// Dikkat: İlişkili randevular varsa silme işlemi başarısız olabilir.
        /// </summary>
        /// <param name="hizmet">Silinecek hizmet</param>
        public async Task RemoveAsync(Hizmet hizmet)
        {
            _context.Hizmetler.Remove(hizmet);
            await _context.SaveChangesAsync();
        }
    }
}