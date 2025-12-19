using System.Linq.Expressions;
using FitnessCenter.Models;

namespace FitnessCenter.Repositories
{
    public interface IRandevuRepository
    {
        Task<List<Randevu>> GetByUserAsync(string userId);
        Task<List<Randevu>> GetByUserIdAsync(string userId); // Alias için
        Task<List<Randevu>> GetByAntrenorAndDateAsync(int antrenorId, DateTime date);
        Task<Randevu?> GetByIdAsync(int id);
        Task<List<Randevu>> GetAllWithDetailsAsync();
        Task AddAsync(Randevu randevu);
        Task RemoveAsync(Randevu randevu);
        Task DeleteAsync(Randevu randevu); // Alias için
        Task UpdateAsync(Randevu randevu);
        Task<bool> AnyAsync(Expression<Func<Randevu, bool>> predicate);
        Task<int> SaveChangesAsync(); // <-- EKLE

    }
}