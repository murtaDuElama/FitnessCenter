// =============================================================================
// DOSYA: RandevuRepository.cs
// AÇIKLAMA: Randevu repository implementasyonu - Entity Framework ile veri erişimi
// NAMESPACE: FitnessCenter.Repositories
// PATTERN: Repository Pattern - IRandevuRepository'nin EF Core implementasyonu
// =============================================================================

using System.Linq.Expressions;
using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Repositories
{
    /// <summary>
    /// Randevu repository implementasyonu.
    /// IRandevuRepository interface'ini Entity Framework Core ile uygular.
    /// AppDbContext üzerinden Randevular tablosuna erişim sağlar.
    /// </summary>
    public class RandevuRepository : IRandevuRepository
    {
        /// <summary>
        /// Entity Framework DbContext - veritabanı bağlantısı
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// RandevuRepository constructor.
        /// </summary>
        /// <param name="context">AppDbContext (DI ile alınır)</param>
        public RandevuRepository(AppDbContext context)
        {
            _context = context;
        }

        // ===================== OKUMA İŞLEMLERİ =====================

        /// <summary>
        /// Kullanıcının tüm randevularını ilişkili verilerle getirir.
        /// Tarihe göre azalan sırada döner.
        /// </summary>
        /// <param name="userId">Kullanıcı ID</param>
        /// <returns>Kullanıcının randevuları</returns>
        public Task<List<Randevu>> GetByUserAsync(string userId)
        {
            return _context.Randevular
                .Include(r => r.Hizmet)      // Hizmet bilgilerini dahil et
                .Include(r => r.Antrenor)    // Antrenör bilgilerini dahil et
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Tarih)
                .ToListAsync();
        }

        /// <summary>
        /// GetByUserAsync için alias metod.
        /// Bazı service'lerde farklı isimlendirme kullanılmakta.
        /// </summary>
        public Task<List<Randevu>> GetByUserIdAsync(string userId)
        {
            return GetByUserAsync(userId);
        }

        /// <summary>
        /// Belirli antrenör ve tarihe ait randevuları getirir.
        /// Müsaitlik kontrolü için kullanılır.
        /// </summary>
        /// <param name="antrenorId">Antrenör ID</param>
        /// <param name="date">Tarih</param>
        /// <returns>O tarihteki antrenör randevuları</returns>
        public Task<List<Randevu>> GetByAntrenorAndDateAsync(int antrenorId, DateTime date)
        {
            return _context.Randevular
                .Where(r => r.AntrenorId == antrenorId && r.Tarih.Date == date.Date)
                .ToListAsync();
        }

        /// <summary>
        /// ID'ye göre randevu getirir.
        /// </summary>
        /// <param name="id">Randevu ID</param>
        /// <returns>Randevu veya null</returns>
        public Task<Randevu?> GetByIdAsync(int id)
        {
            return _context.Randevular.FirstOrDefaultAsync(r => r.Id == id);
        }

        /// <summary>
        /// Tüm randevuları tüm ilişkili verilerle getirir.
        /// Admin panelinde randevu listesi için kullanılır.
        /// </summary>
        /// <returns>Detaylı randevu listesi</returns>
        public Task<List<Randevu>> GetAllWithDetailsAsync()
        {
            return _context.Randevular
                .Include(r => r.Hizmet)      // Hizmet adı ve fiyatı
                .Include(r => r.Antrenor)    // Antrenör adı
                .Include(r => r.User)        // Kullanıcı email
                .OrderByDescending(r => r.Tarih)
                .ToListAsync();
        }

        // ===================== YAZMA İŞLEMLERİ =====================

        /// <summary>
        /// Yeni randevu ekler ve değişiklikleri kaydeder.
        /// </summary>
        /// <param name="randevu">Eklenecek randevu</param>
        public async Task AddAsync(Randevu randevu)
        {
            _context.Randevular.Add(randevu);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Randevuyu veritabanından siler.
        /// </summary>
        /// <param name="randevu">Silinecek randevu</param>
        public async Task RemoveAsync(Randevu randevu)
        {
            _context.Randevular.Remove(randevu);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// RemoveAsync için alias metod.
        /// </summary>
        public async Task DeleteAsync(Randevu randevu)
        {
            await RemoveAsync(randevu);
        }

        /// <summary>
        /// Randevu bilgilerini günceller (örn: onay durumu).
        /// </summary>
        /// <param name="randevu">Güncellenecek randevu</param>
        public async Task UpdateAsync(Randevu randevu)
        {
            _context.Randevular.Update(randevu);
            await _context.SaveChangesAsync();
        }

        // ===================== SORGU İŞLEMLERİ =====================

        /// <summary>
        /// Belirtilen koşula uyan randevu var mı kontrol eder.
        /// Expression ile dinamik sorgu oluşturulabilir.
        /// </summary>
        /// <param name="predicate">Lambda ifadesi ile koşul</param>
        /// <returns>Koşula uyan kayıt varsa true</returns>
        public Task<bool> AnyAsync(Expression<Func<Randevu, bool>> predicate)
        {
            return _context.Randevular.AnyAsync(predicate);
        }

        /// <summary>
        /// Değişiklikleri veritabanına kaydeder.
        /// Unit of Work pattern için kullanılır.
        /// </summary>
        /// <returns>Etkilenen kayıt sayısı</returns>
        public Task<int> SaveChangesAsync()
           => _context.SaveChangesAsync();
    }
}