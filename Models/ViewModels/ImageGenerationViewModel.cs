// =============================================================================
// DOSYA: ImageGenerationViewModel.cs
// AÇIKLAMA: AI görsel üretimi için view model
// NAMESPACE: FitnessCenter.Models.ViewModels
// KULLANIM: ImageGeneration/Index view'ında egzersiz görseli üretmek için
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models.ViewModels
{
    /// <summary>
    /// AI destekli egzersiz görseli üretimi için view model.
    /// Kullanıcının seçtiği egzersiz, gün sayısı ve tekrar bilgilerini toplar.
    /// Bu veriler Pollinations.ai API'sine gönderilerek görsel üretilir.
    /// </summary>
    public class ImageGenerationViewModel
    {
        // ===================== KULLANICI GİRDİLERİ =====================

        /// <summary>
        /// Seçilen egzersizin adı.
        /// Dropdown menüden AvailableExercises listesinden seçilir.
        /// Örnek: "Squat", "Bench Press", "Deadlift"
        /// </summary>
        [Required(ErrorMessage = "Lütfen bir egzersiz seçin.")]
        [Display(Name = "Egzersiz")]
        public string ExerciseName { get; set; } = "";

        /// <summary>
        /// Haftalık antrenman gün sayısı.
        /// Minimum 1, maksimum 7 gün olabilir.
        /// Varsayılan değer: 3 gün
        /// </summary>
        [Required(ErrorMessage = "Lütfen gün sayısı girin.")]
        [Range(1, 7, ErrorMessage = "Gün sayısı 1-7 arasında olmalıdır.")]
        [Display(Name = "Haftalık Gün Sayısı")]
        public int Days { get; set; } = 3;

        /// <summary>
        /// Her sette yapılacak tekrar sayısı.
        /// Minimum 1, maksimum 100 tekrar olabilir.
        /// Varsayılan değer: 10 tekrar
        /// </summary>
        [Required(ErrorMessage = "Lütfen tekrar sayısı girin.")]
        [Range(1, 100, ErrorMessage = "Tekrar sayısı 1-100 arasında olmalıdır.")]
        [Display(Name = "Tekrar Sayısı")]
        public int Reps { get; set; } = 10;

        // ===================== SONUÇ =====================

        /// <summary>
        /// Üretilen görselin URL'i.
        /// Form gönderildikten sonra API yanıtından doldurulur.
        /// null ise henüz görsel üretilmemiş demektir.
        /// </summary>
        public string? GeneratedImageUrl { get; set; }

        // ===================== SABİT LİSTELER =====================

        /// <summary>
        /// Mevcut egzersiz seçenekleri listesi.
        /// Dropdown menüyü doldurmak için kullanılır.
        /// Static property olduğu için instance oluşturmadan erişilebilir.
        /// </summary>
        public static List<string> AvailableExercises => new()
        {
            "Squat",           // Bacak egzersizi
            "Bench Press",     // Göğüs egzersizi
            "Deadlift",        // Sırt/Bacak egzersizi
            "Push-up",         // Göğüs egzersizi (vücut ağırlığı)
            "Pull-up",         // Sırt egzersizi (vücut ağırlığı)
            "Plank",           // Core egzersizi
            "Bicep Curl",      // Kol egzersizi
            "Lunges",          // Bacak egzersizi
            "Shoulder Press",  // Omuz egzersizi
            "Lat Pulldown",    // Sırt egzersizi
            "Leg Press",       // Bacak egzersizi (makine)
            "Tricep Dips",     // Kol egzersizi
            "Crunches",        // Karın egzersizi
            "Burpees",         // Tam vücut egzersizi
            "Kettlebell Swing" // Fonksiyonel egzersiz
        };
    }
}
