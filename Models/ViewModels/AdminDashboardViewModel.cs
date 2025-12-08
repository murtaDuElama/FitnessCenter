public class AdminDashboardViewModel
{
    public int HizmetSayisi { get; set; }
    public int AntrenorSayisi { get; set; }
    public int RandevuSayisi { get; set; }

    public List<Hizmet> SonHizmetler { get; set; }
    public List<Antrenor> SonAntrenorler { get; set; }
    public List<Randevu> SonRandevular { get; set; }
}
