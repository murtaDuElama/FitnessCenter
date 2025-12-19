using System.Threading.Tasks;

namespace FitnessCenter.Services
{
    public interface IGeminiService
    {
        Task<string> GenerateTextAsync(string prompt);
        Task<string> AnalyzeWorkoutAsync(string workoutDescription);
        Task<string> GetNutritionAdviceAsync(string userQuery);
    }
}
