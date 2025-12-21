// =============================================================================
// DOSYA: RandevuController.cs
// AÇIKLAMA: Randevu oluşturma ve yönetim controller'ı
// NAMESPACE: FitnessCenter.Controllers
// ROTALAR: /Randevu/SelectService, /Randevu/SelectTrainer, /Randevu/SelectTime,
//          /Randevu/Create, /Randevu/MyRandevus, /Randevu/IptalEt
// AKIŞ: Hizmet Seç -> Antrenör Seç -> Saat Seç -> Onayla
// =============================================================================

using FitnessCenter.Models;
using FitnessCenter.Repositories;
using FitnessCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenter.Controllers
{
    /// <summary>
    /// Randevu yönetimi controller sınıfı.
    /// Çok adımlı randevu oluşturma akışını yönetir:
    /// 1. Hizmet Seçimi
    /// 2. Antrenör Seçimi
    /// 3. Tarih/Saat Seçimi
    /// 4. Randevu Oluşturma
    /// Ayrıca kullanıcının randevularını listeleme ve iptal etme özelliği sağlar.
    /// </summary>
    public class RandevuController : Controller
    {
        // ===================== DEPENDENCY INJECTION =====================

        /// <summary>Hizmet repository - hizmet verileri için</summary>
        private readonly IHizmetRepository _hizmetRepository;

        /// <summary>Antrenör repository - antrenör verileri için</summary>
        private readonly IAntrenorRepository _antrenorRepository;

        /// <summary>Randevu servisi - randevu iş mantığı için</summary>
        private readonly IRandevuService _randevuService;

        /// <summary>UserManager - kullanıcı bilgilerine erişim için</summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// RandevuController constructor.
        /// Tüm bağımlılıkları DI ile alır.
        /// </summary>
        public RandevuController(
            IHizmetRepository hizmetRepository,
            IAntrenorRepository antrenorRepository,
            IRandevuService randevuService,
            UserManager<ApplicationUser> userManager)
        {
            _hizmetRepository = hizmetRepository;
            _antrenorRepository = antrenorRepository;
            _randevuService = randevuService;
            _userManager = userManager;
        }

        // ===================== ADIM 1: HİZMET SEÇİMİ =====================

        /// <summary>
        /// Hizmet seçimi sayfası - Randevu akışının ilk adımı.
        /// GET: /Randevu/SelectService
        /// Kullanıcıya mevcut hizmetleri listeler.
        /// </summary>
        /// <returns>Hizmet listesi view'ı</returns>
        [HttpGet]
        public async Task<IActionResult> SelectService()
        {
            var hizmetler = await _hizmetRepository.GetAllAsync();
            return View(hizmetler);
        }

        // ===================== ADIM 2: ANTRENÖR SEÇİMİ =====================

        /// <summary>
        /// Antrenör seçimi sayfası - Randevu akışının ikinci adımı.
        /// GET: /Randevu/SelectTrainer?hizmetId={id}
        /// Seçilen hizmete uygun uzmanlık alanındaki antrenörleri listeler.
        /// </summary>
        /// <param name="hizmetId">Seçilen hizmetin ID'si</param>
        /// <returns>Uygun antrenör listesi view'ı veya 404</returns>
        [HttpGet]
        public async Task<IActionResult> SelectTrainer(int hizmetId)
        {
            // Hizmeti getir
            var hizmet = await _hizmetRepository.GetByIdAsync(hizmetId);

            if (hizmet == null)
                return NotFound();

            // Hizmet uzmanlık alanına göre antrenörleri filtrele
            // Örnek: "Fitness Antrenmanı" hizmeti için "Fitness" uzmanlığındaki antrenörler
            var antrenorler = await _antrenorRepository.GetByUzmanlikAsync(hizmet.Ad);

            ViewBag.Hizmet = hizmet;
            return View(antrenorler);
        }

        // ===================== ADIM 3: TARİH/SAAT SEÇİMİ =====================

        /// <summary>
        /// Tarih ve saat seçimi sayfası - Randevu akışının üçüncü adımı.
        /// GET: /Randevu/SelectTime?hizmetId={hId}&antrenorId={aId}&tarih={yyyy-MM-dd}
        /// Antrenörün müsait saatlerini gösterir.
        /// </summary>
        /// <param name="hizmetId">Seçilen hizmetin ID'si</param>
        /// <param name="antrenorId">Seçilen antrenörün ID'si</param>
        /// <param name="tarih">Seçilen tarih (opsiyonel, varsayılan bugün)</param>
        /// <returns>Müsait saatler view'ı</returns>
        [HttpGet]
        public async Task<IActionResult> SelectTime(int hizmetId, int antrenorId, DateTime? tarih)
        {
            var bugun = DateTime.Today;
            var seciliTarih = (tarih?.Date) ?? bugun;

            // Geçmiş tarih kontrolü
            if (seciliTarih < bugun)
            {
                TempData["Error"] = "Geçmiş tarihler için randevu oluşturulamaz. Bugün veya ileri bir tarihi seçiniz.";
                seciliTarih = bugun;
            }

            // Servis üzerinden müsait saatleri al
            // (Antrenörün çalışma saatleri ve dolu randevular hesaplanır)
            var musait = await _randevuService.GetMusaitSaatlerAsync(antrenorId, seciliTarih);

            // ViewBag ile form parametrelerini geçir
            ViewBag.HizmetId = hizmetId;
            ViewBag.AntrenorId = antrenorId;
            ViewBag.SeciliTarih = seciliTarih.ToString("yyyy-MM-dd");
            ViewBag.SeciliTarihLabel = seciliTarih.ToString("dd.MM.yyyy");

            return View(musait);
        }

        // ===================== ADIM 4: RANDEVU OLUŞTURMA =====================

        /// <summary>
        /// Randevu oluşturma işlemi - Akışın son adımı.
        /// POST: /Randevu/Create
        /// Seçilen hizmet, antrenör, tarih ve saat ile randevu kaydeder.
        /// </summary>
        /// <param name="hizmetId">Hizmet ID</param>
        /// <param name="antrenorId">Antrenör ID</param>
        /// <param name="tarih">Randevu tarihi</param>
        /// <param name="saat">Randevu saati (HH:mm)</param>
        /// <returns>Başarıda MyRandevus'a, hatada SelectTime'a yönlendirme</returns>
        [Authorize]  // Giriş yapmış kullanıcı gerekli
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int hizmetId, int antrenorId, DateTime tarih, string saat)
        {
            // Tarih kontrolü
            if (tarih == default)
            {
                TempData["Error"] = "Lütfen bir tarih seçiniz.";
                return RedirectToAction(nameof(SelectTime), new { hizmetId, antrenorId });
            }

            // Saat kontrolü
            if (string.IsNullOrWhiteSpace(saat))
            {
                TempData["Error"] = "Lütfen bir saat seçiniz.";
                return RedirectToAction(nameof(SelectTime), new
                {
                    hizmetId,
                    antrenorId,
                    tarih = tarih.ToString("yyyy-MM-dd")
                });
            }

            // Mevcut kullanıcıyı al
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Randevu oluştur (servis katmanında iş mantığı)
            var (success, error, _) = await _randevuService.CreateAsync(
                user,
                hizmetId,
                antrenorId,
                tarih,
                saat);

            if (!success)
            {
                TempData["Error"] = error ?? "Randevu oluşturulamadı. Lütfen tekrar deneyin.";
                return RedirectToAction(nameof(SelectTime), new
                {
                    hizmetId,
                    antrenorId,
                    tarih = tarih.ToString("yyyy-MM-dd")
                });
            }

            TempData["Success"] = "Randevunuz başarıyla oluşturuldu.";
            return RedirectToAction(nameof(MyRandevus));
        }

        // ===================== RANDEVULARIM =====================

        /// <summary>
        /// Kullanıcının randevuları sayfası.
        /// GET: /Randevu/MyRandevus
        /// Giriş yapmış kullanıcının tüm randevularını listeler.
        /// </summary>
        /// <returns>Kullanıcı randevuları view'ı</returns>
        [HttpGet]
        public async Task<IActionResult> MyRandevus()
        {
            var userId = _userManager.GetUserId(User);
            var randevular = await _randevuService.GetUserRandevularAsync(userId);
            return View(randevular);
        }

        // ===================== RANDEVU İPTAL =====================

        /// <summary>
        /// Randevu iptal etme işlemi.
        /// POST: /Randevu/IptalEt
        /// Kullanıcı sadece kendi randevusunu iptal edebilir.
        /// </summary>
        /// <param name="id">İptal edilecek randevu ID'si</param>
        /// <returns>MyRandevus sayfasına yönlendirme</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IptalEt(int id)
        {
            var userId = _userManager.GetUserId(User);
            var success = await _randevuService.IptalEtAsync(id, userId);

            if (success)
            {
                TempData["Success"] = "Randevu iptal edildi.";
            }
            else
            {
                TempData["Error"] = "Randevu iptal edilemedi.";
            }

            return RedirectToAction(nameof(MyRandevus));
        }

        // ===================== YARDIMCI METODLAR =====================

        /// <summary>
        /// Antrenörün çalışma saatlerinden saat dilimlerini oluşturur.
        /// Her saat başı için slot üretir, öğle arası (12:00) hariç.
        /// </summary>
        /// <param name="antrenor">Antrenör entity</param>
        /// <returns>Saat dilimleri listesi (örn: ["09:00", "10:00", ...])</returns>
        private static List<string> BuildHourlySlots(Antrenor antrenor)
        {
            // Varsayılan çalışma saatleri
            int startHour = 9;
            int endHour = 15;

            // Antrenörün çalışma saatlerini parse et
            if (TimeSpan.TryParse(antrenor.CalismaBaslangicSaati, out var s))
                startHour = s.Hours;

            if (TimeSpan.TryParse(antrenor.CalismaBitisSaati, out var e))
                endHour = e.Hours;

            // Başlangıç > bitiş ise değiştir
            if (endHour < startHour)
                (startHour, endHour) = (endHour, startHour);

            // Saat slotlarını oluştur
            var result = new List<string>();
            for (int h = startHour; h <= endHour; h++)
            {
                if (h == 12) continue; // Öğle arası - randevu alınmaz
                result.Add(h.ToString("D2") + ":00");
            }

            // Boş kalırsa varsayılan saatleri kullan
            if (result.Count == 0)
                result = new List<string> { "09:00", "10:00", "11:00", "13:00", "14:00", "15:00" };

            return result;
        }
    }
}
