using FitnessCenter.Models;
using FitnessCenter.Repositories;

public class RandevuService
{
    private readonly IRandevuRepository _randevuRepository;

    public RandevuService(IRandevuRepository randevuRepository)
    {
        _randevuRepository = randevuRepository;
    }

    public async Task<(bool Success, string? ErrorMessage, Randevu? Randevu)> CreateAsync(
        ApplicationUser user,
        int hizmetId,
        int antrenorId,
        DateTime tarih,
        string saat)
    {
        if (user == null)
            return (false, "Kullanıcı bilgisi alınamadı.", null);

        if (string.IsNullOrWhiteSpace(saat))
            return (false, "Lütfen bir saat seçiniz.", null);

        // Seçilen günü normalize et (saat kısmını sıfırla)
        var seciliGun = tarih.Date;
        var bugun = DateTime.Today;

        if (seciliGun < bugun)
            return (false, "Geçmiş tarihler için randevu oluşturulamaz.", null);

        // Tarih filtresi: start/end aralığı (Date kullanma!)
        var start = seciliGun;
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
            // Onaylandi vb. alanlar sende varsa burada set edebilirsin
        };

        await _randevuRepository.AddAsync(randevu);
        await _randevuRepository.SaveChangesAsync();

        return (true, null, randevu);
    }
}
