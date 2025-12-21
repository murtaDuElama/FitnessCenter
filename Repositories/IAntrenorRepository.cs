// =============================================================================
// DOSYA: IAntrenorRepository.cs
// AÇIKLAMA: Antrenör repository interface - veri erişim sözleşmesi
// NAMESPACE: FitnessCenter.Repositories
// PATTERN: Repository Pattern - Veritabanı işlemlerini soyutlar
// =============================================================================

namespace FitnessCenter.Repositories
{
    /// <summary>
    /// Antrenör repository interface.
    /// Repository Pattern kullanılarak veritabanı işlemlerini soyutlar.
    /// Controller'lar bu interface üzerinden antrenör verilerine erişir.
    /// </summary>
    /// <remarks>
    /// Avantajları:
    /// - Unit testing için mock objeler oluşturulabilir
    /// - Veritabanı bağımlılığını azaltır
    /// - SOLID prensiplerinden Dependency Inversion'ı uygular
    /// </remarks>
    public interface IAntrenorRepository
    {
        /// <summary>
        /// Tüm antrenörleri getirir.
        /// </summary>
        /// <returns>Antrenör listesi</returns>
        Task<List<Antrenor>> GetAllAsync();

        /// <summary>
        /// ID'ye göre antrenör getirir.
        /// </summary>
        /// <param name="id">Antrenör ID</param>
        /// <returns>Antrenör veya null</returns>
        Task<Antrenor?> GetByIdAsync(int id);

        /// <summary>
        /// Uzmanlık alanına göre antrenörleri filtreler.
        /// </summary>
        /// <param name="uzmanlik">Uzmanlık alanı (örn: "Fitness", "Yoga")</param>
        /// <returns>Filtrelenmiş antrenör listesi</returns>
        Task<List<Antrenor>> GetByUzmanlikAsync(string uzmanlik);

        /// <summary>
        /// Yeni antrenör ekler.
        /// </summary>
        /// <param name="antrenor">Eklenecek antrenör</param>
        Task AddAsync(Antrenor antrenor);

        /// <summary>
        /// Antrenör bilgilerini günceller.
        /// </summary>
        /// <param name="antrenor">Güncellenecek antrenör</param>
        Task UpdateAsync(Antrenor antrenor);

        /// <summary>
        /// Antrenörü siler.
        /// </summary>
        /// <param name="antrenor">Silinecek antrenör</param>
        Task RemoveAsync(Antrenor antrenor);
    }
}
