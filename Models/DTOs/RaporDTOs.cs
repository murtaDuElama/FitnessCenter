// =============================================================================
// DOSYA: RaporDTOs.cs
// AÇIKLAMA: Raporlama sistemi için Data Transfer Objects (DTO) sınıfları
// NAMESPACE: FitnessCenter.Models.DTOs
// KULLANIM: API raporlama endpoint'leri ve Admin raporlama sayfalarında
// =============================================================================

namespace FitnessCenter.Models.DTOs
{
    // =========================================================================
    // RANDEVU RAPORU DTO
    // Randevu detaylarını API üzerinden döndürmek için kullanılır
    // =========================================================================

    /// <summary>
    /// Randevu raporu için DTO sınıfı.
    /// Randevu bilgilerini ilişkili entity detaylarıyla birlikte içerir.
    /// API endpoint'lerinde ve raporlama sayfalarında kullanılır.
    /// </summary>
    public class RandevuRaporDto
    {
        /// <summary>Randevu ID'si</summary>
        public int Id { get; set; }

        /// <summary>Randevu sahibinin ad soyadı</summary>
        public string AdSoyad { get; set; }

        /// <summary>Üyenin email adresi</summary>
        public string UyeEmail { get; set; }

        /// <summary>Randevu tarihi</summary>
        public DateTime Tarih { get; set; }

        /// <summary>Randevu saati (HH:mm formatında)</summary>
        public string Saat { get; set; }

        /// <summary>Onay durumu (true: onaylandı, false: beklemede)</summary>
        public bool Onaylandi { get; set; }

        /// <summary>Hizmetin adı</summary>
        public string HizmetAdi { get; set; }

        /// <summary>Hizmetin fiyatı (TL)</summary>
        public decimal HizmetFiyat { get; set; }

        /// <summary>Antrenörün ad soyadı</summary>
        public string AntrenorAdi { get; set; }

        /// <summary>Antrenörün uzmanlık alanı</summary>
        public string AntrenorUzmanlik { get; set; }
    }

    // =========================================================================
    // İSTATİSTİK RAPORU DTO
    // Genel sistem istatistiklerini toplar - Dashboard ve raporlama için
    // =========================================================================

    /// <summary>
    /// Genel istatistik raporu için DTO sınıfı.
    /// Sistemdeki tüm istatistikleri kategorize edilmiş şekilde içerir.
    /// Admin dashboard'unda ve raporlama API'lerinde kullanılır.
    /// </summary>
    public class IstatistikRaporDto
    {
        // ===================== RANDEVU SAYILARI =====================

        /// <summary>Sistemdeki toplam randevu sayısı</summary>
        public int ToplamRandevu { get; set; }

        /// <summary>Bugünkü randevu sayısı</summary>
        public int BugunRandevu { get; set; }

        /// <summary>Bu haftaki randevu sayısı</summary>
        public int BuHaftaRandevu { get; set; }

        /// <summary>Bu ayki randevu sayısı</summary>
        public int BuAyRandevu { get; set; }

        /// <summary>Onaylanmış randevu sayısı</summary>
        public int OnayliRandevu { get; set; }

        /// <summary>Onay bekleyen randevu sayısı</summary>
        public int BekleyenRandevu { get; set; }

        // ===================== GELİR BİLGİLERİ =====================

        /// <summary>Toplam gelir (TL) - tüm zamanlar</summary>
        public decimal ToplamGelir { get; set; }

        /// <summary>Bu ayki gelir (TL)</summary>
        public decimal BuAyGelir { get; set; }

        // ===================== DETAYLI İSTATİSTİKLER =====================

        /// <summary>Hizmet bazında istatistik listesi</summary>
        public List<HizmetIstatistikDto> HizmetBazindaIstatistikler { get; set; }

        /// <summary>Antrenör bazında istatistik listesi</summary>
        public List<AntrenorIstatistikDto> AntrenorBazindaIstatistikler { get; set; }

        /// <summary>Aylık trend verileri (grafik için)</summary>
        public List<AylikIstatistikDto> AylikTrend { get; set; }
    }

    // =========================================================================
    // HİZMET İSTATİSTİK DTO
    // Her hizmet için özet istatistikler
    // =========================================================================

    /// <summary>
    /// Hizmet bazında istatistik DTO'su.
    /// Her hizmetin randevu sayısı ve gelir bilgilerini içerir.
    /// </summary>
    public class HizmetIstatistikDto
    {
        /// <summary>Hizmetin adı</summary>
        public string HizmetAdi { get; set; }

        /// <summary>Bu hizmet için toplam randevu sayısı</summary>
        public int RandevuSayisi { get; set; }

        /// <summary>Bu hizmetten elde edilen toplam gelir (TL)</summary>
        public decimal ToplamGelir { get; set; }

        /// <summary>Ortalama seans fiyatı (TL)</summary>
        public decimal OrtalamaFiyat { get; set; }
    }

    // =========================================================================
    // ANTRENÖR İSTATİSTİK DTO
    // Her antrenör için performans metrikleri
    // =========================================================================

    /// <summary>
    /// Antrenör bazında istatistik DTO'su.
    /// Her antrenörün performans metriklerini içerir.
    /// </summary>
    public class AntrenorIstatistikDto
    {
        /// <summary>Antrenörün ad soyadı</summary>
        public string AntrenorAdi { get; set; }

        /// <summary>Antrenörün uzmanlık alanı</summary>
        public string Uzmanlik { get; set; }

        /// <summary>Toplam randevu sayısı</summary>
        public int RandevuSayisi { get; set; }

        /// <summary>Onaylanmış randevu sayısı</summary>
        public int OnayliRandevuSayisi { get; set; }

        /// <summary>Doluluk oranı (yüzde olarak)</summary>
        public decimal DolulukOrani { get; set; }
    }

    // =========================================================================
    // AYLIK İSTATİSTİK DTO
    // Aylık trend analizleri için (grafik verileri)
    // =========================================================================

    /// <summary>
    /// Aylık istatistik DTO'su.
    /// Trend grafikleri ve aylık karşılaştırmalar için kullanılır.
    /// </summary>
    public class AylikIstatistikDto
    {
        /// <summary>Yıl (örn: 2024)</summary>
        public int Yil { get; set; }

        /// <summary>Ay numarası (1-12)</summary>
        public int Ay { get; set; }

        /// <summary>Ay adı (örn: "Ocak", "Şubat")</summary>
        public string AyAdi { get; set; }

        /// <summary>O ayki randevu sayısı</summary>
        public int RandevuSayisi { get; set; }

        /// <summary>O ayki toplam gelir (TL)</summary>
        public decimal Gelir { get; set; }
    }

    // =========================================================================
    // ANTRENÖR DETAY RAPORU DTO
    // Tek bir antrenör için detaylı performans raporu
    // =========================================================================

    /// <summary>
    /// Antrenör detaylı rapor DTO'su.
    /// Belirli bir antrenörün detaylı performans analizini içerir.
    /// </summary>
    public class AntrenorDetayRaporDto
    {
        /// <summary>Antrenör ID'si</summary>
        public int AntrenorId { get; set; }

        /// <summary>Antrenörün ad soyadı</summary>
        public string AdSoyad { get; set; }

        /// <summary>Uzmanlık alanı</summary>
        public string Uzmanlik { get; set; }

        /// <summary>Toplam randevu sayısı</summary>
        public int ToplamRandevu { get; set; }

        /// <summary>Onaylanan randevu sayısı</summary>
        public int OnayliRandevu { get; set; }

        /// <summary>İptal edilen randevu sayısı</summary>
        public int IptalEdilen { get; set; }

        /// <summary>Çalıştığı tarihler listesi</summary>
        public List<DateTime> MesaiTarihleri { get; set; }

        /// <summary>En çok tercih edilen saat dilimleri</summary>
        public List<string> EnCokTercihEdilenSaatler { get; set; }

        /// <summary>Son randevular listesi</summary>
        public List<RandevuRaporDto> SonRandevular { get; set; }
    }

    // =========================================================================
    // GELİR RAPORU DTO
    // Belirli tarih aralığı için gelir analizi
    // =========================================================================

    /// <summary>
    /// Gelir raporu DTO'su.
    /// Belirli bir tarih aralığındaki gelir analizini içerir.
    /// </summary>
    public class GelirRaporDto
    {
        /// <summary>Rapor başlangıç tarihi</summary>
        public DateTime BaslangicTarihi { get; set; }

        /// <summary>Rapor bitiş tarihi</summary>
        public DateTime BitisTarihi { get; set; }

        /// <summary>Dönem toplam geliri (TL)</summary>
        public decimal ToplamGelir { get; set; }

        /// <summary>Dönem toplam randevu sayısı</summary>
        public int ToplamRandevu { get; set; }

        /// <summary>Ortalama seans başına gelir (TL)</summary>
        public decimal OrtalamaSeans { get; set; }

        /// <summary>Günlük gelir detayları</summary>
        public List<GunlukGelirDto> GunlukDetay { get; set; }

        /// <summary>Hizmet bazında gelir dağılımı</summary>
        public List<HizmetGelirDto> HizmetBazinda { get; set; }
    }

    /// <summary>
    /// Günlük gelir DTO'su.
    /// Her gün için randevu sayısı ve gelir bilgisi içerir.
    /// </summary>
    public class GunlukGelirDto
    {
        /// <summary>Tarih</summary>
        public DateTime Tarih { get; set; }

        /// <summary>O günkü randevu sayısı</summary>
        public int RandevuSayisi { get; set; }

        /// <summary>O günkü toplam gelir (TL)</summary>
        public decimal Gelir { get; set; }
    }

    /// <summary>
    /// Hizmet gelir DTO'su.
    /// Her hizmetin gelire katkısını gösterir.
    /// </summary>
    public class HizmetGelirDto
    {
        /// <summary>Hizmet adı</summary>
        public string HizmetAdi { get; set; }

        /// <summary>Satış adedi</summary>
        public int Miktar { get; set; }

        /// <summary>Birim fiyat (TL)</summary>
        public decimal Fiyat { get; set; }

        /// <summary>Toplam gelir (Miktar x Fiyat)</summary>
        public decimal ToplamGelir { get; set; }
    }

    // =========================================================================
    // RAPOR FİLTRE DTO
    // Raporlama API'leri için filtreleme parametreleri
    // =========================================================================

    /// <summary>
    /// Rapor filtreleme için istek modeli.
    /// API endpoint'lerine gönderilen filtre parametrelerini içerir.
    /// </summary>
    public class RaporFiltreDto
    {
        /// <summary>Başlangıç tarihi filtresi (opsiyonel)</summary>
        public DateTime? BaslangicTarihi { get; set; }

        /// <summary>Bitiş tarihi filtresi (opsiyonel)</summary>
        public DateTime? BitisTarihi { get; set; }

        /// <summary>Hizmet ID filtresi (opsiyonel)</summary>
        public int? HizmetId { get; set; }

        /// <summary>Antrenör ID filtresi (opsiyonel)</summary>
        public int? AntrenorId { get; set; }

        /// <summary>Sadece onaylanan randevuları getir (opsiyonel)</summary>
        public bool? SadeceOnaylananlar { get; set; }

        /// <summary>Üye email filtresi (opsiyonel)</summary>
        public string? UyeEmail { get; set; }
    }
}