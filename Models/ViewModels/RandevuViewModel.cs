// =============================================================================
// DOSYA: RandevuViewModel.cs
// AÇIKLAMA: Randevu listeleme için view model
// NAMESPACE: FitnessCenter.Models.ViewModels
// KULLANIM: Randevu listelerinde okunabilir veri göstermek için
// =============================================================================

namespace FitnessCenter.Models.ViewModels
{
    /// <summary>
    /// Randevu listeleme ve görüntüleme için view model.
    /// Entity'deki ID referansları yerine okunabilir isimler içerir.
    /// Admin paneli ve kullanıcı randevu listelerinde kullanılır.
    /// </summary>
    public class RandevuViewModel
    {
        /// <summary>
        /// Randevunun benzersiz kimlik numarası.
        /// Düzenleme ve silme işlemlerinde kullanılır.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Randevu sahibinin ad ve soyadı.
        /// </summary>
        public string AdSoyad { get; set; } = string.Empty;

        // ===================== HİZMET BİLGİLERİ =====================

        /// <summary>
        /// Hizmetin adı (entity'den çözümlenmiş).
        /// Örnek: "Fitness Antrenmanı", "Yoga Dersi"
        /// </summary>
        public string Hizmet { get; set; } = string.Empty;

        /// <summary>
        /// Hizmetin süresi dakika cinsinden.
        /// </summary>
        public int HizmetSuresi { get; set; }

        /// <summary>
        /// Hizmetin ücreti TL cinsinden.
        /// </summary>
        public decimal Ucret { get; set; }

        // ===================== ANTRENÖR BİLGİSİ =====================

        /// <summary>
        /// Antrenörün ad ve soyadı (entity'den çözümlenmiş).
        /// </summary>
        public string Antrenor { get; set; } = string.Empty;

        // ===================== TARİH VE SAAT =====================

        /// <summary>
        /// Randevunun tarihi.
        /// </summary>
        public DateTime Tarih { get; set; }

        /// <summary>
        /// Randevunun saati.
        /// Format: "HH:mm"
        /// </summary>
        public string Saat { get; set; } = string.Empty;

        // ===================== DURUM =====================

        /// <summary>
        /// Randevunun onay durumu.
        /// true: Onaylandı, false: Beklemede
        /// </summary>
        public bool Onaylandi { get; set; }
    }
}
