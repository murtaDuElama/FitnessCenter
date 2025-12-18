namespace FitnessCenter.Repositories
{
    public interface IAntrenorRepository
    {
        Task<List<Antrenor>> GetAllAsync();
        Task<Antrenor?> GetByIdAsync(int id);
        Task<List<Antrenor>> GetByUzmanlikAsync(string uzmanlik);
        Task AddAsync(Antrenor antrenor);
        Task UpdateAsync(Antrenor antrenor);
        Task RemoveAsync(Antrenor antrenor);
    }
}
