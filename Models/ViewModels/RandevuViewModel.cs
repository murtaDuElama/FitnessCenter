namespace FitnessCenter.Models.ViewModels
{
    public class RandevuViewModel
    {
        public int Id { get; set; }

        public string AdSoyad { get; set; } = string.Empty;

        public string Hizmet { get; set; } = string.Empty;
        public int HizmetSuresi { get; set; }
        public decimal Ucret { get; set; }

        public string Antrenor { get; set; } = string.Empty;

        public DateTime Tarih { get; set; }
        public string Saat { get; set; } = string.Empty;

        public bool Onaylandi { get; set; }
    }
}
