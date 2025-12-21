// =============================================================================
// DOSYA: RandevuService.cs
// AÇIKLAMA: Randevu servisi implementasyonu - iş mantığı ve kurallar
// NAMESPACE: FitnessCenter.Services
// KULLANIM: RandevuController tarafından randevu işlemleri için
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessCenter.Models;
using FitnessCenter.Repositories;

namespace FitnessCenter.Services
{
    /// <summary>
    /// Randevu servisi implementasyonu.
    /// IRandevuService interface'ini uygular.
    /// Randevu iş kurallarını içerir: müsaitlik, çakışma kontrolü vb.
    /// </summary>
    public class RandevuService : IRandevuService
    {
        /// <summary>Randevu repository - veri erişimi için</summary>
        private readonly IRandevuRepository _randevuRepository;

        /// <summary>
        /// Varsayılan saat aralıkları.
        /// Antrenörlerin müsait olduğu saat dilimleri.
        /// 12:00 öğle arası olduğu için listede yok.
        /// </summary>
        private static readonly List<string> SaatAraliklari = new()
        {
            "09:00", "10:00", "11:00",  // Sabah saatleri
            "13:00", "14:00", "15:00",  // Öğleden sonra
            "16:00", "17:00", "18:00"   // Akşam saatleri
        };

        /// <summary>
        /// RandevuService constructor.
        /// </summary>
        /// <param name="randevuRepository">Randevu repository (DI ile)</param>
        public RandevuService(IRandevuRepository randevuRepository)
        {
            _randevuRepository = randevuRepository;
        }

        // ===================== MÜSAİTLİK KONTROLÜ =====================

        /// <summary>
        /// Belirli antrenör ve tarih için müsait saatleri hesaplar.
        /// Dolu randevuları filtreleyerek sadece boş saatleri döner.
        /// </summary>
        /// <param name="antrenorId">Antrenör ID</param>
        /// <param name="date">Kontrol edilecek tarih</param>
        /// <returns>Müsait saat listesi</returns>
        public async Task<List<string>> GetMusaitSaatlerAsync(int antrenorId, DateTime date)
        {
            // Bu tarihte antrenörün dolu randevularını getir
            var doluRandevular = await _randevuRepository.GetByAntrenorAndDateAsync(antrenorId, date);

            // O(1) karmaşıklık için HashSet kullan
            var doluSaatler = new HashSet<string>(
                doluRandevular
                    .Where(r => !string.IsNullOrWhiteSpace(r.Saat))
                    .Select(r => r.Saat!),
                StringComparer.OrdinalIgnoreCase);

            // Dolu olmayan saatleri döndür
            return SaatAraliklari
                .Where(saat => !doluSaatler.Contains(saat))
                .ToList();
        }

        // ===================== RANDEVU OLUŞTURMA =====================

        /// <summary>
        /// Yeni randevu oluşturur.
        /// İş kurallarını kontrol eder:
        /// - Kullanıcı kontrolü
        /// - Geçmiş tarih kontrolü
        /// - Antrenör müsaitlik kontrolü
        /// - Kullanıcı çakışma kontrolü
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage, Randevu? Randevu)> CreateAsync(
            ApplicationUser user,
            int hizmetId,
            int antrenorId,
            DateTime tarih,
            string saat)
        {
            // Kullanıcı kontrolü
            if (user == null)
                return (false, "Kullanıcı bilgisi alınamadı.", null);

            // Saat kontrolü
            if (string.IsNullOrWhiteSpace(saat))
                return (false, "Lütfen bir saat seçiniz.", null);

            var seciliGun = tarih.Date;
            var bugun = DateTime.Today;

            // Geçmiş tarih kontrolü
            if (seciliGun < bugun)
                return (false, "Geçmiş tarihler için randevu oluşturulamaz.", null);

            // Tarih aralığı (günün başı-sonu)
            var start = seciliGun;
            var end = start.AddDays(1);

            // Antrenör müsaitlik kontrolü
            var antrenorDolu = await _randevuRepository.AnyAsync(x =>
                x.AntrenorId == antrenorId &&
                x.Tarih >= start && x.Tarih < end &&
                x.Saat == saat);

            if (antrenorDolu)
                return (false, "Bu saat dolu! Lütfen başka bir saat seçiniz.", null);

            // Kullanıcı çakışma kontrolü
            var kullaniciDolu = await _randevuRepository.AnyAsync(x =>
                x.UserId == user.Id &&
                x.Tarih >= start && x.Tarih < end &&
                x.Saat == saat);

            if (kullaniciDolu)
                return (false, "Bu saatte zaten başka bir randevunuz var!", null);

            // Randevu oluştur
            var randevu = new Randevu
            {
                UserId = user.Id,
                AdSoyad = user.AdSoyad ?? user.UserName ?? "Kullanıcı",
                HizmetId = hizmetId,
                AntrenorId = antrenorId,
                Tarih = start,
                Saat = saat.Trim(),
                Onaylandi = false  // Varsayılan: onay bekliyor
            };

            // Veritabanına kaydet
            await _randevuRepository.AddAsync(randevu);

            return (true, null, randevu);
        }

        // ===================== RANDEVU LİSTELEME =====================

        /// <summary>
        /// Kullanıcının tüm randevularını getirir.
        /// </summary>
        /// <param name="userId">Kullanıcı ID</param>
        /// <returns>Randevu listesi</returns>
        public async Task<List<Randevu>> GetUserRandevularAsync(string userId)
        {
            return await _randevuRepository.GetByUserIdAsync(userId);
        }

        // ===================== RANDEVU İPTAL =====================

        /// <summary>
        /// Randevuyu iptal eder.
        /// Sadece randevu sahibi iptal edebilir (güvenlik kontrolü).
        /// </summary>
        /// <param name="id">Randevu ID</param>
        /// <param name="userId">İşlemi yapan kullanıcı ID</param>
        /// <returns>Başarılıysa true</returns>
        public async Task<bool> IptalEtAsync(int id, string userId)
        {
            var randevu = await _randevuRepository.GetByIdAsync(id);

            // Randevu bulunamadı
            if (randevu == null)
                return false;

            // Güvenlik: Sadece randevu sahibi iptal edebilir
            if (randevu.UserId != userId)
                return false;

            // Randevuyu sil
            await _randevuRepository.DeleteAsync(randevu);
            return true;
        }
    }
}