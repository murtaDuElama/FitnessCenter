using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Repositories
{
    public class HizmetRepository : IHizmetRepository
    {
        private readonly AppDbContext _context;

        public HizmetRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Hizmet>> GetAllAsync()
        {
            return _context.Hizmetler.ToListAsync();
        }

        public Task<Hizmet?> GetByIdAsync(int id)
        {
            return _context.Hizmetler.FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task AddAsync(Hizmet hizmet)
        {
            _context.Hizmetler.Add(hizmet);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Hizmet hizmet)
        {
            _context.Hizmetler.Update(hizmet);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Hizmet hizmet)
        {
            _context.Hizmetler.Remove(hizmet);
            await _context.SaveChangesAsync();
        }
    }
}
