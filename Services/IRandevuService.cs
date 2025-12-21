// =============================================================================
// DOSYA: IRandevuService.cs
// AÇIKLAMA: Randevu servisi interface - randevu iş mantığı için sözleşme
// NAMESPACE: FitnessCenter.Services
// KULLANIM: RandevuController tarafından randevu oluşturma ve yönetimi için
// =============================================================================

using FitnessCenter.Models;

namespace FitnessCenter.Services
{
    /// <summary>
    /// Randevu servisi interface.
    /// Randevu oluşturma, müsaitlik kontrolü ve yönetim işlemleri için
    /// iş mantığı sözleşmesi tanımlar.
    /// </summary>
    /// <remarks>
    /// Repository'den farklı olarak, bu servis iş kurallarını içerir:
    /// - Müsaitlik kontrolü
    /// - Çakışma tespiti
    /// - Geçmiş tarih kontrolü
    /// </remarks>
    public interface IRandevuService
    {
        /// <summary>
        /// Belirli antrenör ve tarih için müsait saatleri getirir.
        /// Dolu saatler filtrelenerek sadece boş saatler döner.
        /// </summary>
        /// <param name="antrenorId">Antrenör ID</param>
        /// <param name="tarih">Kontrol edilecek tarih</param>
        /// <returns>Müsait saat listesi (örn: ["09:00", "10:00", ...])</returns>
        Task<List<string>> GetMusaitSaatlerAsync(int antrenorId, DateTime tarih);

        /// <summary>
        /// Yeni randevu oluşturur.
        /// İş kurallarını kontrol eder ve uygunsa randevuyu kaydeder.
        /// </summary>
        /// <param name="user">Randevuyu oluşturan kullanıcı</param>
        /// <param name="hizmetId">Seçilen hizmet ID</param>
        /// <param name="antrenorId">Seçilen antrenör ID</param>
        /// <param name="tarih">Randevu tarihi</param>
        /// <param name="saat">Randevu saati (HH:mm)</param>
        /// <returns>
        /// Tuple sonuç:
        /// - Success: İşlem başarılı mı
        /// - ErrorMessage: Hata mesajı (başarısızsa)
        /// - Randevu: Oluşturulan randevu (başarılıysa)
        /// </returns>
        Task<(bool Success, string? ErrorMessage, Randevu? Randevu)> CreateAsync(
            ApplicationUser user,
            int hizmetId,
            int antrenorId,
            DateTime tarih,
            string saat);

        /// <summary>
        /// Kullanıcının tüm randevularını getirir.
        /// </summary>
        /// <param name="userId">Kullanıcı ID</param>
        /// <returns>Kullanıcının randevu listesi</returns>
        Task<List<Randevu>> GetUserRandevularAsync(string userId);

        /// <summary>
        /// Randevuyu iptal eder (siler).
        /// Sadece randevu sahibi iptal edebilir.
        /// </summary>
        /// <param name="id">İptal edilecek randevu ID</param>
        /// <param name="userId">İşlemi yapan kullanıcı ID</param>
        /// <returns>İşlem başarılıysa true</returns>
        Task<bool> IptalEtAsync(int id, string userId);
    }
}
