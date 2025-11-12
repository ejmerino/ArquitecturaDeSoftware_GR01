using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ClienteWebEurekabank.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
