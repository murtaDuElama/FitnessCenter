// =============================================================================
// DOSYA: AiResponseModel.cs
// AÇIKLAMA: AI antrenman önerisi için yanıt modeli
// NAMESPACE: FitnessCenter.Models
// KULLANIM: Groq AI servisinden dönen verileri yapılandırılmış şekilde tutar
// =============================================================================

namespace FitnessCenter.Models
{
    /// <summary>
    /// AI destekli antrenman önerisi yanıt modeli.
    /// Groq AI servisinden dönen kişiselleştirilmiş program bilgilerini içerir.
    /// Bu veriler Ai/Index view'ında kullanıcıya gösterilir.
    /// </summary>
    public class AIResponseModel
    {
        /// <summary>
        /// Programın başlık metni.
        /// Örnek: "7 Günlük Kas Geliştirme Programı"
        /// </summary>
        public string Headline { get; set; } = string.Empty;

        /// <summary>
        /// Haftalık antrenman programı listesi.
        /// Her eleman bir günün antrenman detayını içerir.
        /// Örnek: ["Pazartesi: Göğüs ve Triceps", "Salı: Sırt ve Biceps"]
        /// </summary>
        public List<string> WorkoutPlan { get; set; } = new();

        /// <summary>
        /// Beslenme önerileri listesi.
        /// Kullanıcının hedefine uygun diyet tavsiyeleri içerir.
        /// Örnek: ["Günde 2L su için", "Protein ağırlıklı beslenin"]
        /// </summary>
        public List<string> NutritionPlan { get; set; } = new();

        // ===================== BMI BİLGİLERİ =====================

        /// <summary>
        /// Kullanıcının Vücut Kitle İndeksi (BMI) değeri.
        /// Formül: Kilo (kg) / Boy (m)²
        /// </summary>
        public double Bmi { get; set; }

        /// <summary>
        /// BMI kategorisi açıklaması.
        /// Örnek değerler: "Normal", "Fazla Kilolu", "Obez"
        /// </summary>
        public string? BmiCategory { get; set; }

        // ===================== EK BİLGİLER =====================

        /// <summary>
        /// Kullanıcıya özel kişiselleştirilmiş ipucu.
        /// AI tarafından kullanıcının durumuna göre oluşturulur.
        /// </summary>
        public string? PersonalizedTip { get; set; }

        /// <summary>
        /// Ek not veya uyarı mesajı.
        /// Örnek: "Antrenman öncesi ısınma yapmayı unutmayın!"
        /// </summary>
        public string? ExtraNote { get; set; }
    }
}
