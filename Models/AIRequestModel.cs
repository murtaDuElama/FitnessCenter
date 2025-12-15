using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Models
{
    public class AIRequestModel
    {
        [Required(ErrorMessage = "Lütfen fitness hedefinizi seçin.")]
        [Display(Name = "Fitness Hedefi")]
        public string FitnessGoal { get; set; } = string.Empty;

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
