// =============================================================================
// DOSYA: AdminDashboardViewModel.cs
// AÇIKLAMA: Admin paneli dashboard için view model
// NAMESPACE: Global (FitnessCenter.Models.ViewModels altında kullanılabilir)
// KULLANIM: Admin/Home/Index view'ında dashboard istatistikleri göstermek için
// =============================================================================

/// <summary>
/// Admin paneli dashboard view modeli.
/// Admin ana sayfasında gösterilecek özet istatistikleri ve 
/// son kayıtları barındırır.
/// </summary>
public class AdminDashboardViewModel
{
    // ===================== SAYAÇLAR =====================
    // Dashboard kartlarında gösterilecek toplam sayılar
    // ====================================================

    /// <summary>
    /// Sistemdeki toplam hizmet sayısı.
    /// Dashboard'da "Toplam Hizmet" kartında gösterilir.
    /// </summary>
    public int HizmetSayisi { get; set; }

    /// <summary>
    /// Sistemdeki toplam antrenör sayısı.
    /// Dashboard'da "Toplam Antrenör" kartında gösterilir.
    /// </summary>
    public int AntrenorSayisi { get; set; }

    /// <summary>
    /// Sistemdeki toplam randevu sayısı.
    /// Dashboard'da "Toplam Randevu" kartında gösterilir.
    /// </summary>
    public int RandevuSayisi { get; set; }

    // ===================== SON KAYITLAR =====================
    // Dashboard tablolarında gösterilecek son eklenen kayıtlar
    // ========================================================

    /// <summary>
    /// Son eklenen hizmetler listesi.
    /// Dashboard'da "Son Hizmetler" tablosunda gösterilir.
    /// </summary>
    public List<Hizmet> SonHizmetler { get; set; }

    /// <summary>
    /// Son eklenen antrenörler listesi.
    /// Dashboard'da "Son Antrenörler" tablosunda gösterilir.
    /// </summary>
    public List<Antrenor> SonAntrenorler { get; set; }

    /// <summary>
    /// Son oluşturulan randevular listesi.
    /// Dashboard'da "Son Randevular" tablosunda gösterilir.
    /// </summary>
    public List<Randevu> SonRandevular { get; set; }
}
