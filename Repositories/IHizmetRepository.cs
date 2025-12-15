using FitnessCenter.Models;

namespace FitnessCenter.Repositories
{
    public interface IHizmetRepository
    {
        Task<List<Hizmet>> GetAllAsync();
        Task<Hizmet?> GetByIdAsync(int id);
        Task AddAsync(Hizmet hizmet);
        Task UpdateAsync(Hizmet hizmet);
        Task RemoveAsync(Hizmet hizmet);
    }
}
