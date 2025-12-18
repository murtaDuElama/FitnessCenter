namespace FitnessCenter.Models.DTOs
{
    // Randevu Raporu için DTO
    public class RandevuRaporDto
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; }
        public string UyeEmail { get; set; }
        public DateTime Tarih { get; set; }
        public string Saat { get; set; }
        public bool Onaylandi { get; set; }
        public string HizmetAdi { get; set; }
        public decimal HizmetFiyat { get; set; }
        public string AntrenorAdi { get; set; }
        public string AntrenorUzmanlik { get; set; }
    }

    // İstatistik Raporu için DTO
    public class IstatistikRaporDto
    {
        public int ToplamRandevu { get; set; }
        public int BugunRandevu { get; set; }
        public int BuHaftaRandevu { get; set; }
        public int BuAyRandevu { get; set; }
        public int OnayliRandevu { get; set; }
        public int BekleyenRandevu { get; set; }
        public decimal ToplamGelir { get; set; }
        public decimal BuAyGelir { get; set; }
        public List<HizmetIstatistikDto> HizmetBazindaIstatistikler { get; set; }
        public List<AntrenorIstatistikDto> AntrenorBazindaIstatistikler { get; set; }
        public List<AylikIstatistikDto> AylikTrend { get; set; }
    }

    // Hizmet bazında istatistik
    public class HizmetIstatistikDto
    {
        public string HizmetAdi { get; set; }
        public int RandevuSayisi { get; set; }
        public decimal ToplamGelir { get; set; }
        public decimal OrtalamaFiyat { get; set; }
    }

    // Antrenör bazında istatistik
    public class AntrenorIstatistikDto
    {
        public string AntrenorAdi { get; set; }
        public string Uzmanlik { get; set; }
        public int RandevuSayisi { get; set; }
        public int OnayliRandevuSayisi { get; set; }
        public decimal DolulukOrani { get; set; }
    }

    // Aylık trend için DTO
    public class AylikIstatistikDto
    {
        public int Yil { get; set; }
        public int Ay { get; set; }
        public string AyAdi { get; set; }
        public int RandevuSayisi { get; set; }
        public decimal Gelir { get; set; }
    }

    // Antrenör detaylı rapor
    public class AntrenorDetayRaporDto
    {
        public int AntrenorId { get; set; }
        public string AdSoyad { get; set; }
        public string Uzmanlik { get; set; }
        public int ToplamRandevu { get; set; }
        public int OnayliRandevu { get; set; }
        public int IptalEdilen { get; set; }
        public List<DateTime> MesaiTarihleri { get; set; }
        public List<string> EnCokTercihEdilenSaatler { get; set; }
        public List<RandevuRaporDto> SonRandevular { get; set; }
    }

    // Gelir raporu için DTO
    public class GelirRaporDto
    {
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public decimal ToplamGelir { get; set; }
        public int ToplamRandevu { get; set; }
        public decimal OrtalamaSeans { get; set; }
        public List<GunlukGelirDto> GunlukDetay { get; set; }
        public List<HizmetGelirDto> HizmetBazinda { get; set; }
    }

    public class GunlukGelirDto
    {
        public DateTime Tarih { get; set; }
        public int RandevuSayisi { get; set; }
        public decimal Gelir { get; set; }
    }

    public class HizmetGelirDto
    {
        public string HizmetAdi { get; set; }
        public int Miktar { get; set; }
        public decimal Fiyat { get; set; }
        public decimal ToplamGelir { get; set; }
    }

    // Filtreleme için request model
    public class RaporFiltreDto
    {
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int? HizmetId { get; set; }
        public int? AntrenorId { get; set; }
        public bool? SadeceOnaylananlar { get; set; }
        public string? UyeEmail { get; set; }
    }
}