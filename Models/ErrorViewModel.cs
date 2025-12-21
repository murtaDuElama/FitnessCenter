// =============================================================================
// DOSYA: ErrorViewModel.cs
// AÇIKLAMA: Hata sayfası için view model - HTTP hata durumlarını görüntüler
// NAMESPACE: FitnessCenter.Models
// KULLANIM: Error.cshtml view'ı tarafından kullanılır
// =============================================================================

namespace FitnessCenter.Models
{
    /// <summary>
    /// Hata sayfası için view model sınıfı.
    /// Uygulama hatası oluştuğunda Error view'ına gönderilir.
    /// Request ID ile hata takibi yapılabilir.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// HTTP isteğinin benzersiz kimlik numarası.
        /// Hata ayıklama ve log takibi için kullanılır.
        /// Activity.Current?.Id veya HttpContext.TraceIdentifier'dan alınır.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Request ID'nin görüntülenip görüntülenmeyeceğini belirler.
        /// RequestId null veya boş değilse true döner.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
