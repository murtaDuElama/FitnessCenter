using Microsoft.AspNetCore.Mvc;

public class AiController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Analyze()
    {
        // Sonraki aşamada bu yönteme OpenAI kodu ekleyeceğiz.
        return View("Result");
    }

    public IActionResult Result()
    {
        return View();
    }
}
