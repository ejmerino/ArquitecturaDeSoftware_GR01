using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ClienteWebEurekabank.Models;

namespace ClienteWebEurekabank.Controllers
{
    public class MovimientosController : Controller
    {
        private readonly HttpClient _httpClient;

        public MovimientosController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index(string numeroCuenta)
        {
            List<Movimiento> movimientos = new List<Movimiento>();
            var url = "https://localhost:5278/api/EurekabankControlador/Movimientos";

            if (!string.IsNullOrEmpty(numeroCuenta))
            {
                url += $"?numeroCuenta={numeroCuenta}";
            }

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseModel<Movimiento>>(jsonString);
                movimientos = result.Response;
            }
            else
            {
                ViewBag.ErrorMessage = "Error al obtener los movimientos";
            }

            ViewBag.NumeroCuenta = numeroCuenta;
            return View(movimientos);
        }
    }
}
