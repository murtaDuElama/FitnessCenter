using FitnessCenter.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessCenter.Services
{
    public interface IRandevuService
    {
        Task<List<string>> GetMusaitSaatlerAsync(int antrenorId, DateTime tarih);

        Task<(bool Success, string? ErrorMessage, Randevu? Randevu)> CreateAsync(
            ApplicationUser user,
            int hizmetId,
            int antrenorId,
            DateTime tarih,
            string saat);

        Task<List<Randevu>> GetUserRandevularAsync(string userId);

        Task<bool> IptalEtAsync(int id, string userId);
    }
}
