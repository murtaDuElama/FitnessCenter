# LINQ Tabanlý REST API Örnekleri

Bu proje LINQ sorgularýný doðrudan API denetleyicileri içinde kullanýr. Aþaðýdaki örnekler, nerede ve nasýl filtreleme yaptýðýnýzý gösterir.

## Antrenör API'si (Controllers/Api/AntrenorApiController.cs)

- **Tüm antrenörleri listele:** `/api/antrenorler`
- **Uzmanlýða göre filtrele:** `/api/antrenorler?uzmanlik=pilates`
- **Belirli bir tarihte uygun antrenörler:** `/api/antrenorler/uygun?tarih=2024-06-01&saat=10:00`

Kodda LINQ kullanýmý:
```csharp
var query = _context.Antrenorler.AsQueryable();

if (!string.IsNullOrWhiteSpace(uzmanlik))
{
    query = query.Where(a => a.Uzmanlik.Contains(uzmanlik));
}

var antrenorler = await query
    .OrderBy(a => a.AdSoyad)
    .Select(a => new
    {
        a.Id,
        a.AdSoyad,
        a.Uzmanlik,
        a.FotografUrl
    })
    .ToListAsync();
