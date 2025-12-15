using System.Collections.Generic;

namespace FitnessCenter.Models
{
    public class AIResponseModel
    {
        public string Headline { get; set; }
        public List<string> WorkoutPlan { get; set; } = new();
        public List<string> NutritionPlan { get; set; } = new();
        public string? ExtraNote { get; set; }
    }
}
