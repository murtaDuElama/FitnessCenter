// =============================================================================
// DOSYA: AntrenorRepository.cs
// AÇIKLAMA: Antrenör repository implementasyonu - Entity Framework ile veri erişimi
// NAMESPACE: FitnessCenter.Repositories
// PATTERN: Repository Pattern - IAntrenorRepository'nin EF Core implementasyonu
// =============================================================================

using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Repositories
{
    /// <summary>
    /// Antrenör repository implementasyonu.
    /// IAntrenorRepository interface'ini Entity Framework Core ile uygular.
    /// AppDbContext üzerinden Antrenorler tablosuna erişim sağlar.
    /// </summary>
    public class AntrenorRepository : IAntrenorRepository
    {
        /// <summary>
        /// Entity Framework DbContext - veritabanı bağlantısı
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// AntrenorRepository constructor.
        /// </summary>
        /// <param name="context">AppDbContext (DI ile alınır)</param>
        public AntrenorRepository(AppDbContext context)
        {
            _context = context;
        }

        // ===================== OKUMA İŞLEMLERİ =====================

        /// <summary>
        /// Tüm antrenörleri veritabanından getirir.
        /// </summary>
        /// <returns>Antrenör listesi</returns>
        public Task<List<Antrenor>> GetAllAsync()
        {
            return _context.Antrenorler.ToListAsync();
        }

        /// <summary>
        /// ID'ye göre antrenör getirir.
        /// </summary>
        /// <param name="id">Antrenör ID</param>
        /// <returns>Antrenör veya null</returns>
        public Task<Antrenor?> GetByIdAsync(int id)
        {
            return _context.Antrenorler.FirstOrDefaultAsync(a => a.Id == id);
        }

        /// <summary>
        /// Uzmanlık alanına göre antrenörleri filtreler.
        /// Randevu oluşturma akışında hizmete uygun antrenörleri bulmak için kullanılır.
        /// </summary>
        /// <param name="uzmanlik">Uzmanlık alanı</param>
        /// <returns>Filtrelenmiş antrenör listesi</returns>
        public Task<List<Antrenor>> GetByUzmanlikAsync(string uzmanlik)
        {
            return _context.Antrenorler
                .Where(a => a.Uzmanlik == uzmanlik)
                .ToListAsync();
        }

        // ===================== YAZMA İŞLEMLERİ =====================

        /// <summary>
        /// Yeni antrenör ekler ve değişiklikleri kaydeder.
        /// </summary>
        /// <param name="antrenor">Eklenecek antrenör</param>
        public async Task AddAsync(Antrenor antrenor)
        {
            _context.Antrenorler.Add(antrenor);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Antrenör bilgilerini günceller.
        /// EF Core tracking sorunlarını önlemek için mevcut entity detach edilir.
        /// </summary>
        /// <param name="antrenor">Güncellenecek antrenör</param>
        public async Task UpdateAsync(Antrenor antrenor)
        {
            // Tracking sorununu önle: aynı ID'li tracked entity varsa detach et
            var existingEntry = _context.ChangeTracker.Entries<Antrenor>()
                .FirstOrDefault(e => e.Entity.Id == antrenor.Id);

            if (existingEntry != null)
            {
                _context.Entry(existingEntry.Entity).State = EntityState.Detached;
            }

            _context.Antrenorler.Update(antrenor);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Antrenörü veritabanından siler.
        /// </summary>
        /// <param name="antrenor">Silinecek antrenör</param>
        public async Task RemoveAsync(Antrenor antrenor)
        {
            _context.Antrenorler.Remove(antrenor);
            await _context.SaveChangesAsync();
        }
    }
}