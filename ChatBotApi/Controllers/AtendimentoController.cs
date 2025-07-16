using Microsoft.AspNetCore.Mvc;

namespace ChatBotApi.Controllers
{
    public class AtendimentoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
