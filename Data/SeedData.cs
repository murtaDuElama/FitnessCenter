using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Data
{
    public class SeedData
    {

        public static void Seed(AppDbContext context)
        {
            context.Database.Migrate();

            // Hizmet yoksa örnek ekle
            // Belirsizliği gidermek için DbSet'e tam ad ile erişin
            if (!context.Set<Hizmet>().Any())
            {
                context.Set<Hizmet>().AddRange(
                    new Hizmet { Ad = "Fitness", Sure = 60, Ucret = 250 },
                    new Hizmet { Ad = "Yoga", Sure = 45, Ucret = 200 },
                    new Hizmet { Ad = "Pilates", Sure = 45, Ucret = 200 }
                );
            }

            // Antrenör yoksa örnek ekle
            // Antrenör eklerken de aynı şekilde
            if (!context.Set<Antrenor>().Any())
            {
                context.Set<Antrenor>().AddRange(
                    new Antrenor { AdSoyad = "Ahmet Yılmaz", Uzmanlik = "Fitness" },
                    new Antrenor { AdSoyad = "Ayşe Kaya", Uzmanlik = "Yoga" }
                );
            }

            context.SaveChanges();
        }  
    }
}
