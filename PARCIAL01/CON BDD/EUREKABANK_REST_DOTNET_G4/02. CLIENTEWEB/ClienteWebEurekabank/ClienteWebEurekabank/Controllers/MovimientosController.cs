using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ClienteWebEurekabank.Models;
using System;
using System.Globalization;

namespace ClienteWebEurekabank.Controllers
{
    public class MovimientosController : Controller
    {
        private readonly IHttpClientFactory _http;

        public MovimientosController(IHttpClientFactory http)
        {
            _http = http;
        }

        // LISTAR MOVIMIENTOS
        public async Task<IActionResult> Index(string numeroCuenta)
        {
            var client = _http.CreateClient("EurekabankClient");
            var url = "/api/EurekabankControlador/Movimientos";
            if (!string.IsNullOrWhiteSpace(numeroCuenta))
                url += $"?numeroCuenta={Uri.EscapeDataString(numeroCuenta)}";

            List<Movimiento> movimientos = new();
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseModel<Movimiento>>(jsonString);
                movimientos = result?.Response ?? new List<Movimiento>();
            }
            else
            {
                ViewBag.ErrorMessage = $"Error al obtener los movimientos: {(int)response.StatusCode}";
            }

            ViewBag.NumeroCuenta = numeroCuenta;
            ViewBag.Flash = TempData["Flash"];

            // ⚠️ Asegura que renderice la vista correcta de Movimientos
            return View("~/Views/Movimientos/Index.cshtml", movimientos);
        }

        // ---------------- DEPÓSITO ----------------
        [HttpGet("Deposito")]
        public IActionResult Deposito()
        {
            // ⚠️ Renderiza la vista de Depósito y pásale OperacionModel
            return View("~/Views/Deposito/Index.cshtml", new OperacionModel());
        }

        [HttpPost("Deposito")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposito(OperacionModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NumeroCuenta) || model.Valor <= 0)
            {
                ModelState.AddModelError(string.Empty, "Número de cuenta y valor (>0) son obligatorios.");
                return View("~/Views/Deposito/Index.cshtml", model);  // ⚠️ mismo model
            }

            var client = _http.CreateClient("EurekabankClient");
            var url = $"/api/EurekabankControlador/Movimientos/Deposito" +
                      $"?numeroCuenta={Uri.EscapeDataString(model.NumeroCuenta)}&valor={model.Valor.ToString(CultureInfo.InvariantCulture)}";

            var resp = await client.PostAsync(url, null);

            if (resp.IsSuccessStatusCode)
            {
                TempData["Flash"] = "Depósito realizado correctamente.";
                // 🔁 Volver a la lista: esa vista sí recibe List<Movimiento>
                return RedirectToAction(nameof(Index), new { numeroCuenta = model.NumeroCuenta });
            }

            model.Mensaje = $"Error: {await resp.Content.ReadAsStringAsync()}";
            return View("~/Views/Deposito/Index.cshtml", model);
        }

        // ---------------- RETIRO ----------------
        [HttpGet("Retiro")]
        public IActionResult Retiro()
        {
            return View("~/Views/Retiro/Index.cshtml", new OperacionModel());
        }

        [HttpPost("Retiro")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Retiro(OperacionModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NumeroCuenta) || model.Valor <= 0)
            {
                ModelState.AddModelError(string.Empty, "Número de cuenta y valor (>0) son obligatorios.");
                return View("~/Views/Retiro/Index.cshtml", model);
            }

            var client = _http.CreateClient("EurekabankClient");
            var url = $"/api/EurekabankControlador/Movimientos/Retiro" +
                      $"?numeroCuenta={Uri.EscapeDataString(model.NumeroCuenta)}&valor={model.Valor.ToString(CultureInfo.InvariantCulture)}";

            var resp = await client.PostAsync(url, null);

            if (resp.IsSuccessStatusCode)
            {
                TempData["Flash"] = "Retiro realizado correctamente.";
                return RedirectToAction(nameof(Index), new { numeroCuenta = model.NumeroCuenta });
            }

            model.Mensaje = $"Error: {await resp.Content.ReadAsStringAsync()}";
            return View("~/Views/Retiro/Index.cshtml", model);
        }


        // Envoltorio de respuesta del backend (mensaje + lista)
        public class ResponseModel<T>
        {
            [JsonProperty("mensaje")]
            public string Mensaje { get; set; }

            [JsonProperty("response")]
            public List<T> Response { get; set; }
        }
    }
}