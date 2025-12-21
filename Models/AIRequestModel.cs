// =============================================================================
// DOSYA: AIRequestModel.cs
// AÇIKLAMA: AI antrenman önerisi için istek modeli
// NAMESPACE: FitnessCenter.Models
// KULLANIM: AiController tarafından kullanıcı verilerini toplamak için kullanılır
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models
{
    /// <summary>
    /// AI destekli antrenman önerisi için istek modeli.
    /// Kullanıcının fiziksel özelliklerini ve hedeflerini toplar.
    /// Bu veriler Groq AI servisine gönderilerek kişiselleştirilmiş 
    /// antrenman ve beslenme programı oluşturulur.
    /// </summary>
    public class AIRequestModel
    {
        // ===================== FİTNESS HEDEFİ =====================

        /// <summary>
        /// Kullanıcının fitness hedefi.
        /// Örnek değerler: "Kilo Vermek", "Kas Kazanmak", "Dayanıklılık Artırmak"
        /// </summary>
        [Required(ErrorMessage = "Lütfen fitness hedefinizi seçin.")]
        [Display(Name = "Fitness Hedefi")]
        public string FitnessGoal { get; set; } = string.Empty;

        // ===================== FİZİKSEL ÖZELLİKLER =====================

        /// <summary>
        /// Kullanıcının vücut tipi.
        /// Örnek değerler: "Ektomorf", "Mezomorf", "Endomorf"
        /// Antrenman programının şekillendirilmesinde kullanılır.
        /// </summary>
        [Required(ErrorMessage = "Lütfen vücut tipinizi girin.")]
        [Display(Name = "Vücut Tipi")]
        public string BodyType { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcının boyu santimetre cinsinden.
        /// BMI hesaplamasında kullanılır.
        /// </summary>
        [Range(100, 250, ErrorMessage = "Boy 100 - 250 cm arasında olmalıdır.")]
        [Display(Name = "Boy (cm)")]
        public double HeightCm { get; set; }

        /// <summary>
        /// Kullanıcının kilosu kilogram cinsinden.
        /// BMI hesaplamasında kullanılır.
        /// </summary>
        [Range(30, 300, ErrorMessage = "Kilo 30 - 300 kg arasında olmalıdır.")]
        [Display(Name = "Kilo (kg)")]
        public double WeightKg { get; set; }

        /// <summary>
        /// Kullanıcının yaşı.
        /// Antrenman yoğunluğu ve beslenme önerilerinin ayarlanmasında kullanılır.
        /// </summary>
        [Range(12, 100, ErrorMessage = "Yaş 12 - 100 aralığında olmalıdır.")]
        [Display(Name = "Yaş")]
        public int Age { get; set; }

        // ===================== ANTRENMAN TERCİHLERİ =====================

        /// <summary>
        /// Günlük antrenman süresi dakika cinsinden.
        /// Program oluşturulurken dikkate alınır.
        /// </summary>
        [Range(10, 240, ErrorMessage = "Günlük süre 10 - 240 dakika arasında olmalıdır.")]
        [Display(Name = "Günlük Spor Süresi (dk)")]
        public int DailyMinutes { get; set; }

        /// <summary>
        /// Kullanıcının özel notları veya kısıtlamaları.
        /// Örnek: "Diz sakatlığım var", "Vejetaryenim"
        /// </summary>
        [StringLength(500, ErrorMessage = "Notlar 500 karakteri geçemez.")]
        [Display(Name = "Özel Notlar")]
        public string? SpecialNotes { get; set; }

        /// <summary>
        /// Tercih edilen antrenman yoğunluğu.
        /// Örnek değerler: "Düşük", "Orta", "Yüksek"
        /// </summary>
        [Required(ErrorMessage = "Lütfen antrenman yoğunluğunu seçin.")]
        [Display(Name = "Tercih Edilen Yoğunluk")]
        public string Intensity { get; set; } = string.Empty;
    }
}
