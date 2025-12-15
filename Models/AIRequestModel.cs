using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models
{
    public class AIRequestModel
    {
        [Required(ErrorMessage = "Lütfen fitness hedefinizi seçin.")]
        [Display(Name = "Fitness Hedefi")]
        public string FitnessGoal { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen vücut tipinizi girin.")]
        [Display(Name = "Vücut Tipi")]
        public string BodyType { get; set; } = string.Empty;

        [Range(100, 250, ErrorMessage = "Boy 100 - 250 cm arasında olmalıdır.")]
        [Display(Name = "Boy (cm)")]
        public double HeightCm { get; set; }

        [Range(30, 300, ErrorMessage = "Kilo 30 - 300 kg arasında olmalıdır.")]
        [Display(Name = "Kilo (kg)")]
        public double WeightKg { get; set; }

        [Range(12, 100, ErrorMessage = "Yaş 12 - 100 aralığında olmalıdır.")]
        [Display(Name = "Yaş")]
        public int Age { get; set; }

        [Range(10, 240, ErrorMessage = "Günlük süre 10 - 240 dakika arasında olmalıdır.")]
        [Display(Name = "Günlük Spor Süresi (dk)")]
        public int DailyMinutes { get; set; }

        [StringLength(500, ErrorMessage = "Notlar 500 karakteri geçemez.")]
        [Display(Name = "Özel Notlar")]
        public string? SpecialNotes { get; set; }

        [Required(ErrorMessage = "Lütfen antrenman yoğunluğunu seçin.")]
        [Display(Name = "Tercih Edilen Yoğunluk")]
        public string Intensity { get; set; } = string.Empty;
    }
}
