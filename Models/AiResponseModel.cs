namespace FitnessCenter.Models
{
    public class AIResponseModel
    {
        public string Headline { get; set; } = string.Empty;

        public List<string> WorkoutPlan { get; set; } = new();

        public List<string> NutritionPlan { get; set; } = new();

        public double Bmi { get; set; }

        public string? BmiCategory { get; set; }

        public string? PersonalizedTip { get; set; }

        public string? ExtraNote { get; set; }
    }
}
