using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ClienteWebEurekabank.Models;

namespace ClienteWebEurekabank.Controllers
{
    public class CuentasController : Controller
    {
        private readonly HttpClient _httpClient;

        public CuentasController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index(string numeroCuenta)
        {
            List<Cuenta> cuentas = new List<Cuenta>();
            var url = "https://localhost:5278/api/EurekabankControlador/Cuentas";

            if (!string.IsNullOrEmpty(numeroCuenta))
            {
                url += $"?numeroCuenta={numeroCuenta}";
            }

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ResponseModel<Cuenta>>(jsonString);
                cuentas = result.Response;
            }
            else
            {
                ViewBag.ErrorMessage = "Error al obtener las cuentas";
            }

            ViewBag.NumeroCuenta = numeroCuenta;
            return View(cuentas);
        }
    }
}
