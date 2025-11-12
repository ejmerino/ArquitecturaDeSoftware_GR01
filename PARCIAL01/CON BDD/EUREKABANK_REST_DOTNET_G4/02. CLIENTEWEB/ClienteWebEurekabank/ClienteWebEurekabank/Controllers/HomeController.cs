using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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

        // Login (vista)
        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Views/Home/Index.cshtml (tu formulario de login por nombre)
        }

        // Registro (vista)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Registro (envío)
        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string apellido, string correo, string telefono, string cedula, string rol, string contrasena)
        {
            var client = _httpClientFactory.CreateClient("EurekabankClient");

            // Tu API acepta estos datos como querystring; si luego lo cambias a JSON, avísame y lo ajusto
            var qs = $"?nombre={nombre}&apellido={apellido}&correo={correo}&telefono={telefono}&cedula={cedula}&rol={rol}&contrasena={contrasena}";
            var resp = await client.PostAsync($"/api/EurekabankControlador/Usuarios{qs}", null);

            if (resp.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Usuario registrado con éxito.";
                return View();
            }

            ViewBag.ErrorMessage = "Hubo un error al registrar el usuario.";
            return View();
        }

        // ===== LOGIN POR NOMBRE (NO CORREO) =====
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var client = _httpClientFactory.CreateClient("EurekabankClient");

            // JSON que espera el endpoint /Usuarios/Entrar { nombre, contrasena }
            var payload = new
            {
                nombre = username,
                contrasena = password
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("/api/EurekabankControlador/Usuarios/Entrar", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(jsonString);

                if (json["mensaje"]?.ToString() == "ok")
                {
                    var user = json["response"];

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, user["usuNombre"]?.ToString() ?? ""),
                        new Claim("FullName", $"{user["usuNombre"]} {user["usuApellido"]}"),
                        new Claim(ClaimTypes.Role, user["usuRol"]?.ToString() ?? "")
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // Redirección post-login (puedes personalizar por rol)
                    return RedirectToAction("Index", "Menu");
                }
            }

            ViewBag.ErrorMessage = "Nombre de usuario o contraseña incorrectos.";
            return View("Index");
        }

        // Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }
    }
}
