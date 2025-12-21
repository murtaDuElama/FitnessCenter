# FitnessCenter Proje Raporu

**Proje Adı:** FitnessCenter - Fitness Merkezi Yönetim Sistemi  
**Teknoloji:** ASP.NET Core MVC (.NET 8)  
**Veritabanı:** SQL Server + Entity Framework Core  
**Tarih:** 21 Aralık 2024

---

## 📋 İçindekiler

1. [Proje Tanıtımı](#1-proje-tanıtımı)
2. [Kullanılan Teknolojiler](#2-kullanılan-teknolojiler)
3. [Veritabanı Modeli](#3-veritabanı-modeli)
4. [Sistem Mimarisi](#4-sistem-mimarisi)
5. [Özellikler](#5-özellikler)
6. [Ekran Görüntüleri](#6-ekran-görüntüleri)

---

## 1. Proje Tanıtımı

FitnessCenter, bir fitness merkezi için geliştirilmiş kapsamlı bir yönetim sistemidir. Sistem, üyelerin hizmetleri görüntülemesine, antrenör seçmesine ve randevu almasına olanak tanır. Ayrıca yöneticiler için tam kapsamlı bir admin paneli içerir.

### Projenin Amacı
- Fitness merkezi üyelerinin online randevu alabilmesi
- Hizmetlerin ve antrenörlerin yönetimi
- Randevu onay/iptal süreçlerinin dijitalleştirilmesi
- AI destekli egzersiz görsel üretimi

### Kullanıcı Rolleri
| Rol | Yetkiler |
|-----|----------|
| **Üye (Uye)** | Hizmet görüntüleme, randevu alma, kendi randevularını yönetme |
| **Admin** | Tüm CRUD işlemleri, randevu onaylama, raporlama, kullanıcı yönetimi |

---

## 2. Kullanılan Teknolojiler

### Backend
- **ASP.NET Core MVC** (.NET 8) - Web uygulama framework'ü
- **Entity Framework Core** - ORM (Object-Relational Mapping)
- **ASP.NET Core Identity** - Kimlik doğrulama ve yetkilendirme
- **SQL Server** - Veritabanı

### Frontend
- **Razor Views** - Sunucu taraflı render
- **Bootstrap 5** - CSS framework
- **jQuery** - JavaScript kütüphanesi
- **jQuery Validation** - İstemci taraflı form doğrulama

### API & Entegrasyonlar
- **Swagger/OpenAPI** - REST API dokümantasyonu
- **Groq AI** - Yapay zeka entegrasyonu
- **Pollinations.ai** - AI destekli görsel üretimi

### Mimari Desenler
- **Repository Pattern** - Veri erişim katmanı
- **Dependency Injection** - Bağımlılık yönetimi
- **MVC Pattern** - Model-View-Controller mimarisi
- **Areas** - Admin paneli ayrımı

---

## 3. Veritabanı Modeli

### 3.1 Entity-Relationship Diyagramı

```
┌─────────────────┐       ┌─────────────────┐       ┌─────────────────┐
│  ApplicationUser│       │     Randevu     │       │    Antrenor     │
├─────────────────┤       ├─────────────────┤       ├─────────────────┤
│ Id (PK)         │◄──────│ UserId (FK)     │       │ Id (PK)         │
│ AdSoyad         │       │ Id (PK)         │───────►│ AdSoyad         │
│ Email           │       │ AdSoyad         │       │ Uzmanlik        │
│ UserName        │       │ HizmetId (FK)   │       │ FotografUrl     │
│ PasswordHash    │       │ AntrenorId (FK) │       │ CalismaBaslangic│
│ ...             │       │ Tarih           │       │ CalismaBitis    │
└─────────────────┘       │ Saat            │       └─────────────────┘
                          │ Onaylandi       │
                          └────────┬────────┘
                                   │
                                   │
                          ┌────────▼────────┐
                          │     Hizmet      │
                          ├─────────────────┤
                          │ Id (PK)         │
                          │ Ad (Unique)     │
                          │ Sure            │
                          │ Ucret           │
                          └─────────────────┘
```

### 3.2 Tablo Detayları

#### ApplicationUser (Kullanıcı)
ASP.NET Identity'den türetilmiş kullanıcı modeli.

| Alan | Tip | Açıklama |
|------|-----|----------|
| Id | string (GUID) | Birincil anahtar |
| AdSoyad | string(100) | Kullanıcının ad soyad bilgisi |
| Email | string | E-posta adresi (benzersiz) |
| UserName | string | Kullanıcı adı |
| PasswordHash | string | Şifrelenmiş parola |

```csharp
public class ApplicationUser : IdentityUser
{
    [StringLength(100)]
    public string? AdSoyad { get; set; }
}
```

---

#### Antrenor (Eğitmen)
Fitness merkezindeki antrenörleri temsil eder.

| Alan | Tip | Açıklama |
|------|-----|----------|
| Id | int | Birincil anahtar (auto-increment) |
| AdSoyad | string | Antrenör ad soyad (zorunlu) |
| Uzmanlik | string | Uzmanlık alanı (zorunlu) |
| FotografUrl | string? | Profil fotoğrafı URL'i |
| CalismaBaslangicSaati | string(5) | Çalışma başlangıç saati (örn: "09:00") |
| CalismaBitisSaati | string(5) | Çalışma bitiş saati (örn: "15:00") |

```csharp
public class Antrenor
{
    public int Id { get; set; }

    [Required]
    public string AdSoyad { get; set; }

    [Required]
    public string Uzmanlik { get; set; }

    public string? FotografUrl { get; set; }

    [Required, StringLength(5)]
    public string CalismaBaslangicSaati { get; set; } = "09:00";

    [Required, StringLength(5)]
    public string CalismaBitisSaati { get; set; } = "15:00";
}
```

---

#### Hizmet
Fitness merkezinin sunduğu hizmetleri temsil eder.

| Alan | Tip | Açıklama |
|------|-----|----------|
| Id | int | Birincil anahtar |
| Ad | string | Hizmet adı (benzersiz, zorunlu) |
| Sure | int | Süre (dakika) |
| Ucret | decimal | Ücret (TL) |

```csharp
public class Hizmet
{
    public int Id { get; set; }

    [Required]
    public string Ad { get; set; }

    [Required]
    public int Sure { get; set; }  // Dakika

    [Required]
    public decimal Ucret { get; set; }
}
```

---

#### Randevu
Üye-Hizmet-Antrenör ilişkisini tutan randevu kaydı.

| Alan | Tip | Açıklama |
|------|-----|----------|
| Id | int | Birincil anahtar |
| AdSoyad | string | Randevu sahibi ad soyad |
| HizmetId | int (FK) | Seçilen hizmet |
| AntrenorId | int (FK) | Seçilen antrenör |
| Tarih | DateTime | Randevu tarihi |
| Saat | string | Randevu saati (örn: "10:00") |
| UserId | string (FK) | Randevuyu alan kullanıcı |
| Onaylandi | bool | Onay durumu (varsayılan: false) |

```csharp
public class Randevu
{
    public int Id { get; set; }

    [Required]
    public string AdSoyad { get; set; }

    [Required]
    public int HizmetId { get; set; }
    public Hizmet Hizmet { get; set; }

    [Required]
    public int AntrenorId { get; set; }
    public Antrenor Antrenor { get; set; }

    [Required]
    public DateTime Tarih { get; set; }

    [Required]
    public string Saat { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public bool Onaylandi { get; set; } = false;
}
```

### 3.3 İlişki Türleri

| İlişki | Tür | Açıklama |
|--------|-----|----------|
| Randevu → Hizmet | Bire-Çok | Her randevu bir hizmete ait |
| Randevu → Antrenor | Bire-Çok | Her randevu bir antrenöre ait |
| Randevu → ApplicationUser | Bire-Çok | Her randevu bir kullanıcıya ait |

---

## 4. Sistem Mimarisi

### 4.1 Katmanlı Mimari

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │   Views     │  │ Controllers │  │    Admin Area       │  │
│  │  (Razor)    │  │   (MVC)     │  │   (Admin Panel)     │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
├─────────────────────────────────────────────────────────────┤
│                    BUSINESS LAYER                            │
│  ┌─────────────────────┐  ┌────────────────────────────┐    │
│  │     Services        │  │     External Services      │    │
│  │  - RandevuService   │  │  - GroqService (AI)        │    │
│  │                     │  │  - PollinationsService     │    │
│  └─────────────────────┘  └────────────────────────────┘    │
├─────────────────────────────────────────────────────────────┤
│                    DATA ACCESS LAYER                         │
│  ┌─────────────────────┐  ┌────────────────────────────┐    │
│  │   Repositories      │  │      Entity Framework      │    │
│  │  - HizmetRepository │  │      Core (AppDbContext)   │    │
│  │  - AntrenorRepository│  │                           │    │
│  │  - RandevuRepository │  │                           │    │
│  └─────────────────────┘  └────────────────────────────┘    │
├─────────────────────────────────────────────────────────────┤
│                    DATABASE LAYER                            │
│  ┌─────────────────────────────────────────────────────┐    │
│  │                   SQL Server                         │    │
│  │    AspNetUsers, Antrenorler, Hizmetler, Randevular  │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

### 4.2 Controller Yapısı

| Controller | Açıklama |
|------------|----------|
| `HomeController` | Ana sayfa ve genel sayfalar |
| `AccountController` | Giriş, kayıt, çıkış işlemleri |
| `HizmetController` | Hizmet listeleme |
| `AntrenorController` | Antrenör listeleme |
| `RandevuController` | Randevu alma ve yönetim |
| `AiController` | AI chat entegrasyonu |
| `ImageGenerationController` | AI görsel üretimi |

### 4.3 Admin Area

Admin paneli `Areas/Admin` altında ayrı bir yapıda organize edilmiştir:

| Controller | Açıklama |
|------------|----------|
| `HomeController` | Admin dashboard |
| `HizmetController` | Hizmet CRUD |
| `AntrenorController` | Antrenör CRUD |
| `RandevuController` | Randevu yönetimi ve onaylama |
| `RaporController` | LINQ ile raporlama |

---

## 5. Özellikler

### 5.1 Kullanıcı Özellikleri

✅ **Kimlik Doğrulama**
- Kayıt olma (e-posta, şifre, ad soyad)
- Giriş yapma
- Çıkış yapma
- Hesap silme

✅ **Hizmet İşlemleri**
- Tüm hizmetleri görüntüleme
- Hizmet detaylarını inceleme (süre, ücret)

✅ **Antrenör İşlemleri**
- Tüm antrenörleri görüntüleme
- Uzmanlık alanlarına göre filtreleme
- Çalışma saatlerini görme

✅ **Randevu İşlemleri**
- Hizmet seçimi
- Antrenör seçimi
- Tarih ve saat seçimi
- Randevu oluşturma
- Kendi randevularını görüntüleme
- Randevu iptali

✅ **AI Özellikleri**
- AI destekli sohbet (Groq)
- Egzersiz görsel üretimi (Pollinations.ai)

### 5.2 Admin Özellikleri

✅ **Dashboard**
- Genel istatistikler
- Bekleyen randevu sayısı
- Aktif üye sayısı

✅ **Hizmet Yönetimi (CRUD)**
- Hizmet ekleme
- Hizmet düzenleme
- Hizmet silme

✅ **Antrenör Yönetimi (CRUD)**
- Antrenör ekleme
- Antrenör düzenleme
- Antrenör silme
- Fotoğraf yükleme

✅ **Randevu Yönetimi**
- Tüm randevuları görüntüleme
- Randevu onaylama
- Randevu silme

✅ **Raporlama (LINQ)**
- Hizmete göre randevu sayıları
- Antrenöre göre randevu sayıları
- Tarih bazlı raporlar

### 5.3 Güvenlik Özellikleri

✅ **Data Validation**
- Sunucu taraflı validation (Data Annotations)
- İstemci taraflı validation (jQuery Validation)

✅ **Yetkilendirme**
- Role-based authorization (Admin, Uye)
- `[Authorize]` attribute'ları

✅ **CSRF Koruması**
- Anti-forgery token kullanımı

---

## 6. Ekran Görüntüleri

> **Not:** Aşağıdaki bölümlere ekran görüntülerinizi ekleyiniz.

### 6.1 Ana Sayfa

<!-- EKRAN GÖRÜNTÜSÜ BURAYA EKLENECEK -->
![Ana Sayfa](screenshots/anasayfa.png)

*Açıklama: Uygulamanın ana sayfası, kullanıcıları karşılayan hoş geldiniz ekranı*

---

### 6.2 Kayıt Sayfası

<!-- EKRAN GÖRÜNTÜSÜ BURAYA EKLENECEK -->
![Kayıt Sayfası](screenshots/kayit.png)

*Açıklama: Yeni kullanıcı kayıt formu (Ad Soyad, E-posta, Şifre, Şifre Tekrar)*

---

### 6.3 Giriş Sayfası

<!-- EKRAN GÖRÜNTÜSÜ BURAYA EKLENECEK -->
![Giriş Sayfası](screenshots/giris.png)

*Açıklama: Kullanıcı giriş formu*

---

### 6.4 Hizmetler Sayfası

<!-- EKRAN GÖRÜNTÜSÜ BURAYA EKLENECEK -->
![Hizmetler](screenshots/hizmetler.png)

*Açıklama: Fitness merkezinin sunduğu hizmetlerin listesi (Fitness, Yoga, Pilates vb.)*

---

### 6.5 Antrenörler Sayfası

<!-- EKRAN GÖRÜNTÜSÜ BURAYA EKLENECEK -->
![Antrenörler](screenshots/antrenorler.png)

*Açıklama: Fitness merkezinde çalışan antrenörlerin listesi*

---

### 6.6 Antrenör Seçimi

<!-- EKRAN GÖRÜNTÜSÜ BURAYA EKLENECEK -->
![Antrenör Seçimi](screenshots/antrenor_secimi.png)

*Açıklama: Randevu oluşturma adımı - antrenör seçimi*

---

### 6.7 Randevu Oluşturma

<!-- EKRAN GÖRÜNTÜSÜ BURAYA EKLENECEK -->
![Randevu Oluşturma](screenshots/randevu_olusturma.png)

*Açıklama: Tarih ve saat seçimi ile randevu oluşturma formu*

---

### 6.8 Randevularım

<!-- EKRAN GÖRÜNTÜSÜ BURAYA EKLENECEK -->
![Randevularım](screenshots/randevularim.png)

*Açıklama: Kullanıcının aldığı randevuların listesi*

---

### 6.9 AI Görsel Üretimi

<!-- EKRAN GÖRÜNTÜSÜ BURAYA EKLENECEK -->
![AI Görsel Üretimi](screenshots/ai_gorsel.png)

*Açıklama: AI destekli egzersiz görseli üretme sayfası*

---

### 6.10 Admin Panel - Dashboard

<!-- EKRAN GÖRÜNTÜSÜ BURAYA EKLENECEK -->
![Admin Dashboard](screenshots/admin_dashboard.png)

*Açıklama: Admin paneli ana ekranı - istatistikler ve özet*

---

### 6.11 Admin Panel - Hizmet Yönetimi

<!-- EKRAN GÖRÜNTÜSÜ BURAYA EKLENECEK -->
![Admin Hizmetler](screenshots/admin_hizmetler.png)

*Açıklama: Admin panelinde hizmet listesi ve CRUD işlemleri*

---

### 6.12 Admin Panel - Antrenör Yönetimi

<!-- EKRAN GÖRÜNTÜSÜ BURAYA EKLENECEK -->
![Admin Antrenörler](screenshots/admin_antrenorler.png)

*Açıklama: Admin panelinde antrenör listesi ve CRUD işlemleri*

---

### 6.13 Admin Panel - Randevu Yönetimi

<!-- EKRAN GÖRÜNTÜSÜ BURAYA EKLENECEK -->
![Admin Randevular](screenshots/admin_randevular.png)

*Açıklama: Admin panelinde randevu onaylama ve yönetimi*

---

### 6.14 Swagger API Dokümantasyonu

<!-- EKRAN GÖRÜNTÜSÜ BURAYA EKLENECEK -->
![Swagger](screenshots/swagger.png)

*Açıklama: REST API dokümantasyonu (/swagger)*

---

### 6.15 Validation Örneği

<!-- EKRAN GÖRÜNTÜSÜ BURAYA EKLENECEK -->
![Validation](screenshots/validation.png)

*Açıklama: Form doğrulama hataları örneği (istemci/sunucu taraflı)*

---

## 7. Kurulum ve Çalıştırma

### Gereksinimler
- .NET 8 SDK
- SQL Server (LocalDB veya Express)
- Visual Studio 2022 veya VS Code

### Adımlar

1. **Projeyi Klonlayın**
   ```bash
   git clone [repository-url]
   cd FitnessCenter
   ```

2. **Veritabanını Oluşturun**
   ```bash
   dotnet ef database update
   ```

3. **Projeyi Çalıştırın**
   ```bash
   dotnet run
   ```

4. **Tarayıcıda Açın**
   - Uygulama: `https://localhost:5001`
   - Swagger: `https://localhost:5001/swagger`

### Varsayılan Kullanıcılar
| E-posta | Şifre | Rol |
|---------|-------|-----|
| admin@fitnesscenter.com | sau | Admin |

---

## 8. Sonuç

FitnessCenter projesi, modern web teknolojileri kullanılarak geliştirilmiş kapsamlı bir fitness merkezi yönetim sistemidir. Proje:

- **MVC Mimarisi** ile temiz kod yapısı
- **Entity Framework Core** ile veritabanı yönetimi
- **ASP.NET Identity** ile güvenli kimlik doğrulama
- **Repository Pattern** ile veri erişim katmanı ayrımı
- **REST API** ile dış entegrasyon desteği
- **AI Entegrasyonu** ile modern özellikler

içermektedir.

---

**Hazırlayan:** [Grup Üyeleri Adları]  
**Tarih:** 21 Aralık 2024  
**Ders:** [Ders Adı]  
**Öğretim Üyesi:** [Hoca Adı]
