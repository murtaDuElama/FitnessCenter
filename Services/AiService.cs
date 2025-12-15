using System;
using System.Collections.Generic;
using System.Linq;
using FitnessCenter.Models;

namespace FitnessCenter.Services
{
    public class AiService
    {
        private static readonly Dictionary<string, string[]> GoalTemplates =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Kilo verme"] = new[]
                {
                    "Haftada 4 gün: 2 gün kardiyo + 2 gün kuvvet antrenmanı",
                    "Her antrenman öncesi 5 dk ısınma, sonrası 10 dk esneme",
                    "Günlük adım hedefi: 8000-10000"
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
            var bodyType = request.BodyType?.Trim();

            string[]? template = null;

            var hasTemplate =
                !string.IsNullOrWhiteSpace(goalInput) &&
                GoalTemplates.TryGetValue(goalInput!, out template);

            var goal = hasTemplate ? goalInput! : "Genel fitness";

            var workout = template != null
                ? template.ToList()
                : new List<string> { "Haftada 3 gün tam vücut antrenmanı", "Düzenli kardiyo ve esneme" };

            var intensity = string.IsNullOrWhiteSpace(request.Intensity) ? "Orta" : request.Intensity.Trim();

            var bmi = CalculateBmi(request.HeightCm, request.WeightKg);
            var bmiCategory = GetBmiCategory(bmi);

            var response = new AIResponseModel
            {
                Headline = $"{goal} hedefi için {request.DailyMinutes} dakikalık plan",
                WorkoutPlan = workout,
                NutritionPlan = BuildNutrition(goal, intensity),
                Bmi = Math.Round(bmi, 1),
                BmiCategory = bmiCategory,
                PersonalizedTip = BuildPersonalizedTip(bodyType, bmiCategory, request.Age, goal),
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
                        "Günlük kaloriyi 300-500 kcal artırın",
                        "Protein + kompleks karbonhidrat ağırlıklı ara öğün ekleyin",
                        "Sıvı kaloriler (smoothie) ile destekleyin"
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

        private static double CalculateBmi(double heightCm, double weightKg)
        {
            if (heightCm <= 0 || weightKg <= 0)
            {
                return 0;
            }

            var heightMeter = heightCm / 100d;
            return weightKg / Math.Pow(heightMeter, 2);
        }

        private static string GetBmiCategory(double bmi)
        {
            if (bmi <= 0)
                return "Belirlenemedi";

            if (bmi < 18.5) return "Zayıf";
            if (bmi < 24.9) return "Normal";
            if (bmi < 29.9) return "Fazla kilolu";
            return "Obez";
        }

        private static string BuildPersonalizedTip(string? bodyType, string bmiCategory, int age, string goal)
        {
            var hints = new List<string>();

            if (!string.IsNullOrWhiteSpace(bodyType))
            {
                hints.Add($"Vücut tipi: {bodyType}");
            }

            if (!string.IsNullOrWhiteSpace(bmiCategory) &&
                !string.Equals(bmiCategory, "Belirlenemedi", StringComparison.OrdinalIgnoreCase))
            {
                hints.Add($"BMI yorumu: {bmiCategory}");
            }

            if (age > 0)
            {
                hints.Add(age < 25
                    ? "Genç yaş grubunda toparlanma hızlıdır, yoğunluk artırılabilir."
                    : "Toparlanma süresine dikkat edin, kaliteli uyku planlayın.");
            }

            if (!string.IsNullOrWhiteSpace(goal))
            {
                hints.Add($"AI modeli, {goal.ToLower()} hedefi için toparlanma ve beslenme dengesini önceliklendirmenizi öneriyor.");
            }

            return hints.Count == 0
                ? "Temel sağlık kontrolü sonrasında plana başlayın."
                : string.Join(" · ", hints);
        }
    }
}
