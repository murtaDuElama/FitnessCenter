// =============================================================================
// DOSYA: IRandevuRepository.cs
// AÇIKLAMA: Randevu repository interface - veri erişim sözleşmesi
// NAMESPACE: FitnessCenter.Repositories
// PATTERN: Repository Pattern - Veritabanı işlemlerini soyutlar
// =============================================================================

using System.Linq.Expressions;
using FitnessCenter.Models;

namespace FitnessCenter.Repositories
{
    /// <summary>
    /// Randevu repository interface.
    /// Repository Pattern kullanılarak veritabanı işlemlerini soyutlar.
    /// Randevu CRUD işlemleri ve özel sorgular için kullanılır.
    /// </summary>
    /// <remarks>
    /// Bu interface standart CRUD'a ek olarak:
    /// - Kullanıcı bazlı randevu sorgulama
    /// - Antrenör ve tarih bazlı sorgulama
    /// - Expression tabanlı AnyAsync sorgusu
    /// gibi özel metodlar içerir.
    /// </remarks>
    public interface IRandevuRepository
    {
        // ===================== OKUMA İŞLEMLERİ =====================

        /// <summary>
        /// Kullanıcının tüm randevularını getirir.
        /// İlişkili Hizmet ve Antrenör bilgileri dahil edilir.
        /// </summary>
        /// <param name="userId">Kullanıcı ID (Identity)</param>
        /// <returns>Kullanıcının randevu listesi</returns>
        Task<List<Randevu>> GetByUserAsync(string userId);

        /// <summary>
        /// GetByUserAsync için alias metod.
        /// </summary>
        /// <param name="userId">Kullanıcı ID</param>
        /// <returns>Kullanıcının randevu listesi</returns>
        Task<List<Randevu>> GetByUserIdAsync(string userId);

        /// <summary>
        /// Belirli antrenör ve tarihe ait randevuları getirir.
        /// Müsaitlik kontrolünde kullanılır.
        /// </summary>
        /// <param name="antrenorId">Antrenör ID</param>
        /// <param name="date">Tarih</param>
        /// <returns>O tarihteki antrenör randevuları</returns>
        Task<List<Randevu>> GetByAntrenorAndDateAsync(int antrenorId, DateTime date);

        /// <summary>
        /// ID'ye göre randevu getirir.
        /// </summary>
        /// <param name="id">Randevu ID</param>
        /// <returns>Randevu veya null</returns>
        Task<Randevu?> GetByIdAsync(int id);

        /// <summary>
        /// Tüm randevuları ilişkili verilerle birlikte getirir.
        /// Admin panelinde kullanılır.
        /// </summary>
        /// <returns>Detaylı randevu listesi (Hizmet, Antrenör, User dahil)</returns>
        Task<List<Randevu>> GetAllWithDetailsAsync();

        // ===================== YAZMA İŞLEMLERİ =====================

        /// <summary>
        /// Yeni randevu ekler.
        /// </summary>
        /// <param name="randevu">Eklenecek randevu</param>
        Task AddAsync(Randevu randevu);

        /// <summary>
        /// Randevuyu siler.
        /// </summary>
        /// <param name="randevu">Silinecek randevu</param>
        Task RemoveAsync(Randevu randevu);

        /// <summary>
        /// RemoveAsync için alias metod.
        /// </summary>
        /// <param name="randevu">Silinecek randevu</param>
        Task DeleteAsync(Randevu randevu);

        /// <summary>
        /// Randevu bilgilerini günceller.
        /// </summary>
        /// <param name="randevu">Güncellenecek randevu</param>
        Task UpdateAsync(Randevu randevu);

        // ===================== SORGU İŞLEMLERİ =====================

        /// <summary>
        /// Belirtilen koşula uyan randevu var mı kontrol eder.
        /// Müsaitlik kontrollerinde kullanılır.
        /// </summary>
        /// <param name="predicate">Lambda ifadesi ile koşul</param>
        /// <returns>Koşula uyan kayıt varsa true</returns>
        Task<bool> AnyAsync(Expression<Func<Randevu, bool>> predicate);

        /// <summary>
        /// Değişiklikleri veritabanına kaydeder.
        /// Unit of Work pattern için kullanılır.
        /// </summary>
        /// <returns>Etkilenen kayıt sayısı</returns>
        Task<int> SaveChangesAsync();
    }
}