using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Repositories
{
    public class AntrenorRepository : IAntrenorRepository
    {
        private readonly AppDbContext _context;

        public AntrenorRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Antrenor>> GetAllAsync()
        {
            return _context.Antrenorler.ToListAsync();
        }

        public Task<Antrenor?> GetByIdAsync(int id)
        {
            return _context.Antrenorler.FirstOrDefaultAsync(a => a.Id == id);
        }

        public Task<List<Antrenor>> GetByUzmanlikAsync(string uzmanlik)
        {
            return _context.Antrenorler
                .Where(a => a.Uzmanlik == uzmanlik)
                .ToListAsync();
        }

        public async Task AddAsync(Antrenor antrenor)
        {
            _context.Antrenorler.Add(antrenor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Antrenor antrenor)
        {
            _context.Antrenorler.Update(antrenor);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Antrenor antrenor)
        {
            _context.Antrenorler.Remove(antrenor);
            await _context.SaveChangesAsync();
        }
    }
}
