using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ClienteWebEurekabank.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string apellido, string correo, string telefono, string cedula, string rol, string contrasena)
        {
            var client = _httpClientFactory.CreateClient("EurekabankClient");
            var queryString = $"?nombre={nombre}&apellido={apellido}&correo={correo}&telefono={telefono}&cedula={cedula}&rol={rol}&contrasena={contrasena}";
            var response = await client.PostAsync($"/api/EurekabankControlador/Usuarios{queryString}", null);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Usuario registrado con éxito.";
                return View();
            }

            ViewBag.ErrorMessage = "Hubo un error al registrar el usuario. Por favor, inténtelo de nuevo.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var client = _httpClientFactory.CreateClient("EurekabankClient");
            var response = await client.GetAsync($"/api/EurekabankControlador/Usuarios/PorCorreo?correo={username}&contrasena={password}");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(responseData);

                if (jsonResponse["mensaje"].ToString() == "ok")
                {
                    var userResponse = jsonResponse["response"];
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, userResponse["usuNombre"].ToString()),
                        new Claim(ClaimTypes.Email, userResponse["usuCorreo"].ToString()),
                        new Claim("FullName", userResponse["usuNombre"] + " " + userResponse["usuApellido"]),
                        new Claim(ClaimTypes.Role, userResponse["usuRol"].ToString())
                    };
                    var identity = new ClaimsIdentity(claims, "login");
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(principal);

                    // Redirigir según el rol del usuario
                    if (userResponse["usuRol"].ToString() == "Administrador")
                    {
                        return RedirectToAction("Index", "Menu");
                    }
                    else if (userResponse["usuRol"].ToString() == "Usuario")
                    {
                        return RedirectToAction("Usuario", "Usuarios");
                    }
                }
            }

            ViewBag.ErrorMessage = "Usuario o contraseña incorrectos.";
            return View("Index");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}
