using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ClienteWebEurekabank.Models; // Asegúrate de que esta línea está presente

namespace ClienteWebEurekabank.Controllers
{
    public class TransferenciasController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseAddress;

        public TransferenciasController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseAddress = "https://localhost:5278/api/EurekabankControlador";
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Transferencia(TransferenciaModel model)
        {
            var requestUri = $"{_baseAddress}/Transferencia?cuentaOrigen={model.CuentaOrigen}&cuentaDestino={model.CuentaDestino}&valor={model.Valor}";

            try
            {
                var response = await _httpClient.PostAsync(requestUri, null);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ResponseModel<string>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    ViewBag.Result = result.Mensaje;
                }
                else
                {
                    ViewBag.Result = "Error al realizar la transferencia.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Result = $"Excepción: {ex.Message}";
            }

            return View("Index");
        }
    }
}
