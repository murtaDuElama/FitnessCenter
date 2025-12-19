using System.Threading.Tasks;

namespace FitnessCenter.Services
{
    public interface IAIService
    {
        Task<string> AnalyzeWorkoutAsync(string workoutDescription);
        Task<string> GetNutritionAdviceAsync(string nutritionQuery);
        Task<string> GenerateTextAsync(string prompt);
    }
}
