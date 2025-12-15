using System;
using System.Collections.Generic;
using System.Linq;
using FitnessCenter.Models;

namespace FitnessCenter.Services
{
    public class AiService
    {
        private static readonly Dictionary<string, string[]> GoalTemplates = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Kilo verme"] = new[]
            {
                "Haftada 3 gün düşük-orta tempo kardiyo (yürüyüş/koşu bandı/bisiklet)",
                "2 gün tüm vücut direnç antrenmanı (hafif ağırlık, yüksek tekrar)",
                "Antrenman sonu 10 dakika core ve esneme"
            },
            ["Kas geliştirme"] = new[]
            {
                "Haftada 4 gün split çalışma: üst vücut / alt vücut dönüşümlü",
                "Temel hareketlerde (squat, bench, deadlift) progresif yükleme",
                "Her antrenman sonrası 10 dakika mobilite"
            },
            ["Esneklik artırma"] = new[]
            {
                "Haftada 3 gün yoga veya pilates",
                "Günde 15 dakikalık germe rutinleri",
                "Düşük tempolu kardiyo ile ısınma"
            },
            ["Kilo alma"] = new[]
            {
                "Haftada 3-4 gün tam vücut güç antrenmanı",
                "Bileşik hareketlere (squat, bench, row) odaklanma",
                "Kısa kardiyo ile toparlanma"
            }
        };
        public AIResponseModel BuildPlan(AIRequestModel request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var goalInput = request.FitnessGoal?.Trim();

            string[]? template = null; // <-- kritik: önce tanımla

            var hasTemplate =
                !string.IsNullOrWhiteSpace(goalInput) &&
                GoalTemplates.TryGetValue(goalInput!, out template);

            var goal = hasTemplate ? goalInput! : "Genel fitness";

            var workout = template != null
                ? template.ToList()
                : new List<string> { "Haftada 3 gün tam vücut antrenmanı", "Düzenli kardiyo ve esneme" };

            var intensity = string.IsNullOrWhiteSpace(request.Intensity) ? "Orta" : request.Intensity.Trim();

            var response = new AIResponseModel
            {
                Headline = $"{goal} hedefi için {request.DailyMinutes} dakikalık plan",
                WorkoutPlan = workout,
                NutritionPlan = BuildNutrition(goal, intensity),
                ExtraNote = string.IsNullOrWhiteSpace(request.SpecialNotes)
                    ? "Plan, sakatlanma riskini azaltmak için 5-10 dakikalık ısınma içerir."
                    : $"Kullanıcı notu dikkate alındı: {request.SpecialNotes.Trim()}"
            };

            response.WorkoutPlan.Insert(0, $"Yoğunluk tercihi: {intensity}");
            response.WorkoutPlan.Add($"Günlük hedef süre: {request.DailyMinutes} dakika");

            return response;
        }

        private static List<string> BuildNutrition(string goal, string intensity)
        {
            var suggestions = new List<string>();

            // GoalTemplates StringComparer.OrdinalIgnoreCase kullandığı için burada da case-insensitive davranmak istiyoruz
            switch (goal?.Trim())
            {
                case string g when string.Equals(g, "Kilo verme", StringComparison.OrdinalIgnoreCase):
                    suggestions.AddRange(new[]
                    {
                        "Günlük kaloriyi %15-20 açığa çekin",
                        "Öğün başına 25-30g protein hedefleyin",
                        "Şekerli içecekleri su veya bitki çayı ile değiştirin"
                    });
                    break;

                case string g when string.Equals(g, "Kilo alma", StringComparison.OrdinalIgnoreCase):
                    suggestions.AddRange(new[]
                    {
                        "Kaloriyi %10-15 artıya alın",
                        "Ara öğünlerde fıstık ezmesi, yoğurt, yulaf tercih edin",
                        "Antrenman sonrası 25g protein + 40g karbonhidrat"
                    });
                    break;

                case string g when string.Equals(g, "Kas geliştirme", StringComparison.OrdinalIgnoreCase):
                    suggestions.AddRange(new[]
                    {
                        "Vücut ağırlığı başına 1.6-2g protein",
                        "Kompleks karbonhidrat ve sağlıklı yağları dengeleyin",
                        "Antrenman günlerinde ek 300-400 kcal"
                    });
                    break;

                default:
                    suggestions.AddRange(new[]
                    {
                        "Günlük 2-2.5L su tüketimi",
                        "Her öğünde sebze/yeşillik ekleyin",
                        "Lif ve protein dengesine dikkat edin"
                    });
                    break;
            }

            var intensityNorm = (intensity ?? "").Trim();

            suggestions.Add(string.Equals(intensityNorm, "Yüksek", StringComparison.OrdinalIgnoreCase)
                ? "Elektrolit desteği ve uyku kalitesine ekstra önem verin"
                : "Yoğunluk düşük/orta ise bol su ve dengeli karbonhidrat yeterlidir");

            return suggestions;
        }
    }
}
