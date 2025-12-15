using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessCenter.Models;
using FitnessCenter.Repositories;

namespace FitnessCenter.Services
{
    public class RandevuService
    {
        private readonly IRandevuRepository _randevuRepository;

        private static readonly List<string> SaatAraliklari = new()
        {
            "09:00","10:00","11:00","13:00","14:00","15:00"
        };

        public RandevuService(IRandevuRepository randevuRepository)
        {
            _randevuRepository = randevuRepository;
        }

        public async Task<List<string>> GetMusaitSaatlerAsync(int antrenorId, DateTime date)
        {
            var doluRandevular = await _randevuRepository.GetByAntrenorAndDateAsync(antrenorId, date);

            // O(1) membership check
            var doluSaatler = new HashSet<string>(
                doluRandevular
                    .Where(r => !string.IsNullOrWhiteSpace(r.Saat))
                    .Select(r => r.Saat!),
                StringComparer.OrdinalIgnoreCase);

            return SaatAraliklari
                .Where(saat => !doluSaatler.Contains(saat))
                .ToList();
        }

        public async Task<(bool Success, string? ErrorMessage, Randevu? Randevu)> CreateAsync(
            ApplicationUser user,
            int hizmetId,
            int antrenorId,
            DateTime tarih,
            string saat)
        {
            if (user == null) return (false, "Kullanıcı bilgisi alınamadı.", null);

            if (string.IsNullOrWhiteSpace(saat))
                return (false, "Lütfen bir saat seçiniz.", null);

            // Tarih filtresi: start/end aralığı (Date kullanma!)
            var start = tarih.Date;
            var end = start.AddDays(1);

            var antrenorDolu = await _randevuRepository.AnyAsync(x =>
                x.AntrenorId == antrenorId &&
                x.Tarih >= start && x.Tarih < end &&
                x.Saat == saat);

            if (antrenorDolu)
                return (false, "Bu saat dolu! Lütfen başka bir saat seçiniz.", null);

            var kullaniciDolu = await _randevuRepository.AnyAsync(x =>
                x.UserId == user.Id &&
                x.Tarih >= start && x.Tarih < end &&
                x.Saat == saat);

            if (kullaniciDolu)
                return (false, "Bu saatte zaten başka bir randevunuz var!", null);

            var randevu = new Randevu
            {
                UserId = user.Id,
                HizmetId = hizmetId,
                AntrenorId = antrenorId,
                Tarih = start,
                Saat = saat.Trim(),
                AdSoyad = user.AdSoyad ?? user.UserName ?? user.Email
            };

            await _randevuRepository.AddAsync(randevu);
            return (true, null, randevu);
        }

        public Task<List<Randevu>> GetUserRandevularAsync(string userId)
        {
            return _randevuRepository.GetByUserAsync(userId);
        }

        public async Task<bool> IptalEtAsync(int id, string userId)
        {
            var randevu = await _randevuRepository.GetByIdAsync(id);

            if (randevu == null || randevu.UserId != userId)
                return false;

            await _randevuRepository.RemoveAsync(randevu);
            return true;
        }
    }
}
