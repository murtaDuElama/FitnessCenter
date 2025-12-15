using System.Linq.Expressions;
using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Repositories
{
    public class RandevuRepository : IRandevuRepository
    {
        private readonly AppDbContext _context;

        public RandevuRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Randevu>> GetByUserAsync(string userId)
        {
            return _context.Randevular
                .Include(r => r.Hizmet)
                .Include(r => r.Antrenor)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Tarih)
                .ToListAsync();
        }

        public Task<List<Randevu>> GetByAntrenorAndDateAsync(int antrenorId, DateTime date)
        {
            return _context.Randevular
                .Where(r => r.AntrenorId == antrenorId && r.Tarih.Date == date.Date)
                .ToListAsync();
        }

        public Task<Randevu?> GetByIdAsync(int id)
        {
            return _context.Randevular.FirstOrDefaultAsync(r => r.Id == id);
        }

        public Task<List<Randevu>> GetAllWithDetailsAsync()
        {
            return _context.Randevular
                .Include(r => r.Hizmet)
                .Include(r => r.Antrenor)
                .Include(r => r.User)
                .OrderByDescending(r => r.Tarih)
                .ToListAsync();
        }

        public async Task AddAsync(Randevu randevu)
        {
            _context.Randevular.Add(randevu);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Randevu randevu)
        {
            _context.Randevular.Remove(randevu);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Randevu randevu)
        {
            _context.Randevular.Update(randevu);
            await _context.SaveChangesAsync();
        }

        public Task<bool> AnyAsync(Expression<Func<Randevu, bool>> predicate)
        {
            return _context.Randevular.AnyAsync(predicate);
        }
    }
}
