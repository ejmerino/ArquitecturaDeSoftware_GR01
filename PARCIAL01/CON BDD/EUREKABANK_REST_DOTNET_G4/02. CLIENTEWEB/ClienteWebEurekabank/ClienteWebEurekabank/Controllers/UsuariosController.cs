using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using ClienteWebEurekabank.Models;
using System.Text;

namespace ClienteWebEurekabank.Controllers
{
    public class UsuariosController : Controller
    {

        public IActionResult Usuario()
        {
            return View();
        }

        // GET: Usuarios/Index
        public async Task<IActionResult> Index(int? id)
        {
            if (!id.HasValue)
            {
                return View(new UsuarioModel());
            }

            var url = $"https://localhost:5278/api/EurekabankControlador/Usuarios/{id.Value}";

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var usuarioResponse = JsonDocument.Parse(jsonString)
                        .RootElement
                        .GetProperty("response")
                        .Deserialize<UsuarioModel>(new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    return View(usuarioResponse);
                }
            }

            return View(new UsuarioModel());
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var url = $"https://localhost:5278/api/EurekabankControlador/Usuarios/{id}";

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var usuarioResponse = JsonDocument.Parse(jsonString)
                        .RootElement
                        .GetProperty("response")
                        .Deserialize<UsuarioModel>(new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    return View("Index", usuarioResponse); // Muestra el formulario de edición en la vista Index
                }
            }

            return NotFound();
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioModel usuario)
        {
            if (id != usuario.UsuId)
            {
                return BadRequest();
            }

            var url = $"https://localhost:5278/api/EurekabankControlador/Usuarios/{id}";

            using (var client = new HttpClient())
            {
                var json = JsonSerializer.Serialize(usuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index), new { id = id }); // Redirige al Index con el ID del usuario actualizado
                }
            }

            return View("Index", usuario); // Muestra el formulario de edición en caso de error
        }
    }
}
