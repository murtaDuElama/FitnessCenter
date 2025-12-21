// =============================================================================
// DOSYA: IHizmetRepository.cs
// AÇIKLAMA: Hizmet repository interface - veri erişim sözleşmesi
// NAMESPACE: FitnessCenter.Repositories
// PATTERN: Repository Pattern - Veritabanı işlemlerini soyutlar
// =============================================================================

namespace FitnessCenter.Repositories
{
    /// <summary>
    /// Hizmet repository interface.
    /// Repository Pattern kullanılarak veritabanı işlemlerini soyutlar.
    /// Controller'lar bu interface üzerinden hizmet verilerine erişir.
    /// </summary>
    /// <remarks>
    /// Bu interface aşağıdaki CRUD operasyonlarını tanımlar:
    /// - Create: AddAsync
    /// - Read: GetAllAsync, GetByIdAsync
    /// - Update: UpdateAsync
    /// - Delete: RemoveAsync
    /// </remarks>
    public interface IHizmetRepository
    {
        /// <summary>
        /// Tüm hizmetleri getirir.
        /// </summary>
        /// <returns>Hizmet listesi</returns>
        Task<List<Hizmet>> GetAllAsync();

        /// <summary>
        /// ID'ye göre hizmet getirir.
        /// </summary>
        /// <param name="id">Hizmet ID</param>
        /// <returns>Hizmet veya null</returns>
        Task<Hizmet?> GetByIdAsync(int id);

        /// <summary>
        /// Yeni hizmet ekler.
        /// </summary>
        /// <param name="hizmet">Eklenecek hizmet</param>
        Task AddAsync(Hizmet hizmet);

        /// <summary>
        /// Hizmet bilgilerini günceller.
        /// </summary>
        /// <param name="hizmet">Güncellenecek hizmet</param>
        Task UpdateAsync(Hizmet hizmet);

        /// <summary>
        /// Hizmeti siler.
        /// </summary>
        /// <param name="hizmet">Silinecek hizmet</param>
        Task RemoveAsync(Hizmet hizmet);
    }
}
