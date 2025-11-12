using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EurekabankClienteConsola
{
    // ==== Modelos mínimos que espera este Program ====
    public class ApiResponse<T>
    {
        public string Mensaje { get; set; }
        public T Response { get; set; }
    }

    public class Usuario
    {
        public int UsuId { get; set; }
        public string UsuNombre { get; set; }
        public string UsuRol { get; set; }
    }

    public class Cuenta
    {
        public int CuId { get; set; }
        public string CuNumero { get; set; }
        public string CuTipo { get; set; }
        public decimal CuSaldo { get; set; }
    }

    public class Movimiento
    {
        public int MovId { get; set; }
        public string MovTipo { get; set; }
        public DateTime MovFecha { get; set; }
        public decimal MovValor { get; set; }
        public decimal MovSaldoFinal { get; set; }
    }

    public class LoginRequest
    {
        public string nombre { get; set; }
        public string contrasena { get; set; }
    }

    class Program
    {
        private static readonly HttpClient _httpClient;

        static Program()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true; // solo DEV

            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri("https://localhost:5278/api/EurekabankControlador/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        static async Task Main(string[] args)
        {
            MostrarTitulo();

            var usuario = await LoginAsyncInteractivo();
            if (usuario == null)
            {
                Console.WriteLine("\nCredenciales inválidas o formato no esperado por el backend.");
                Console.WriteLine("Revisa el mensaje de error que imprimimos arriba (HTTP/JSON).");
                return;
            }

            await MostrarMenuAsync(usuario);
        }

        private static void MostrarTitulo()
        {
            Console.Clear();
            Console.WriteLine(@"
==================================================
|                                                |
|          Bienvenido al Sistema Eurekabank      |
|                                                |
==================================================
");
        }

        // === LOGIN: prueba JSON, form-urlencoded y querystring (C# 7.3) ===
        private static async Task<Usuario> LoginAsyncInteractivo()
        {
            Console.WriteLine("Por favor, ingrese sus credenciales.");
            Console.Write("Nombre de usuario: ");
            string nombre = Console.ReadLine();
            Console.Write("Contraseña: ");
            string contrasena = Console.ReadLine();

            var u1 = await TryLoginJson(nombre, contrasena);
            if (u1 != null) return u1;

            var u2 = await TryLoginFormUrlEncoded(nombre, contrasena);
            if (u2 != null) return u2;

            var u3 = await TryLoginQuery(nombre, contrasena);
            if (u3 != null) return u3;

            return null;
        }

        // ---- Estrategia 1: POST JSON (sin 'using var') ----
        private static async Task<Usuario> TryLoginJson(string nombre, string contrasena)
        {
            try
            {
                var login = new LoginRequest { nombre = nombre, contrasena = contrasena };
                var json = JsonConvert.SerializeObject(login);
                using (var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))
                {
                    var resp = await _httpClient.PostAsync("Usuarios/Entrar", content);
                    if (!resp.IsSuccessStatusCode)
                    {
                        await PrintHttpError("JSON", resp);
                        return null;
                    }

                    var body = await resp.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<ApiResponse<Usuario>>(body);
                    if (data != null && data.Mensaje == "ok" && data.Response != null)
                        return data.Response;

                    PrintApiMensaje("JSON", data != null ? data.Mensaje : null, body);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Login JSON] Excepción: " + ex.Message);
                return null;
            }
        }

        // ---- Estrategia 2: POST application/x-www-form-urlencoded ----
        private static async Task<Usuario> TryLoginFormUrlEncoded(string nombre, string contrasena)
        {
            try
            {
                var formPairs = new[]
                {
                    new KeyValuePair<string,string>("nombre", nombre),
                    new KeyValuePair<string,string>("contrasena", contrasena)
                };

                using (var form = new FormUrlEncodedContent(formPairs))
                {
                    var resp = await _httpClient.PostAsync("Usuarios/Entrar", form);
                    if (!resp.IsSuccessStatusCode)
                    {
                        await PrintHttpError("FORM", resp);
                        return null;
                    }

                    var body = await resp.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<ApiResponse<Usuario>>(body);
                    if (data != null && data.Mensaje == "ok" && data.Response != null)
                        return data.Response;

                    PrintApiMensaje("FORM", data != null ? data.Mensaje : null, body);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Login FORM] Excepción: " + ex.Message);
                return null;
            }
        }

        // ---- Estrategia 3: POST con querystring ----
        private static async Task<Usuario> TryLoginQuery(string nombre, string contrasena)
        {
            try
            {
                var url = "Usuarios/Entrar?nombre=" + Uri.EscapeDataString(nombre) +
                          "&contrasena=" + Uri.EscapeDataString(contrasena);

                var resp = await _httpClient.PostAsync(url, null);
                if (!resp.IsSuccessStatusCode)
                {
                    await PrintHttpError("QUERY", resp);
                    return null;
                }

                var body = await resp.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ApiResponse<Usuario>>(body);
                if (data != null && data.Mensaje == "ok" && data.Response != null)
                    return data.Response;

                PrintApiMensaje("QUERY", data != null ? data.Mensaje : null, body);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Login QUERY] Excepción: " + ex.Message);
                return null;
            }
        }

        private static void PrintApiMensaje(string intento, string mensaje, string raw)
        {
            Console.WriteLine("[Login " + intento + "] API Mensaje: " + (mensaje ?? "(null)"));
            if (!string.IsNullOrWhiteSpace(raw))
            {
                Console.WriteLine("[Login " + intento + "] Cuerpo devuelto:");
                Console.WriteLine(raw);
            }
        }

        private static async Task PrintHttpError(string intento, HttpResponseMessage resp)
        {
            Console.WriteLine("[Login " + intento + "] HTTP " + (int)resp.StatusCode + " " + resp.ReasonPhrase);
            var raw = await resp.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(raw))
            {
                Console.WriteLine("[Login " + intento + "] Cuerpo devuelto:");
                Console.WriteLine(raw);
            }
        }

        // ================= MENÚ =================

        private static async Task MostrarMenuAsync(Usuario usuario)
        {
            bool continuar = true;
            while (continuar)
            {
                Console.Clear();
                MostrarTitulo();
                Console.WriteLine(@"
==================================================
|                                                |
|       Bienvenido, " + usuario.UsuNombre + @"
|                                                |
==================================================
");
                Console.WriteLine("1. Consultar Cuentas");
                Console.WriteLine("2. Consultar Movimientos");
                Console.WriteLine("3. Realizar Depósito");
                Console.WriteLine("4. Realizar Retiro");
                Console.WriteLine("5. Realizar Transferencia");
                Console.WriteLine("6. Cerrar Sesión");
                Console.Write("\nSeleccione una opción: ");

                string op = Console.ReadLine();
                switch (op)
                {
                    case "1":
                        await ConsultarCuentasAsync();
                        break;
                    case "2":
                        await ConsultarMovimientosAsync();
                        break;
                    case "3":
                        await RealizarDepositoAsync();
                        break;
                    case "4":
                        await RealizarRetiroAsync();
                        break;
                    case "5":
                        await RealizarTransferenciaAsync();
                        break;
                    case "6":
                        continuar = false;
                        break;
                    default:
                        Console.WriteLine("\nOpción no válida. Presione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static async Task ConsultarCuentasAsync()
        {
            Console.Clear();
            MostrarTitulo();
            Console.WriteLine("\nConsultar Cuentas");
            Console.WriteLine("=================\n");

            Console.Write("Número de cuenta (deje en blanco para todas): ");
            string numeroCuenta = Console.ReadLine();

            string url = "Cuentas";
            if (!string.IsNullOrEmpty(numeroCuenta))
                url += "?numeroCuenta=" + Uri.EscapeDataString(numeroCuenta);

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Error al obtener las cuentas: " + response.ReasonPhrase);
                Console.ReadKey(); return;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<Cuenta>>>(jsonString);

            if (apiResponse != null && apiResponse.Mensaje == "ok" && apiResponse.Response != null)
            {
                Console.WriteLine("\nCuentas encontradas:");
                Console.WriteLine("==================================================");
                Console.WriteLine(" ID | Número de Cuenta | Tipo   | Saldo");
                Console.WriteLine("==================================================");
                foreach (var c in apiResponse.Response)
                    Console.WriteLine(" " + c.CuId.ToString().PadRight(2) + " | " +
                                      (c.CuNumero ?? "").PadRight(16) + " | " +
                                      (c.CuTipo ?? "").PadRight(6) + " | " +
                                      c.CuSaldo.ToString("C2").PadLeft(10));
                Console.WriteLine("==================================================");
            }
            else
            {
                Console.WriteLine(apiResponse != null ? apiResponse.Mensaje : "Error al obtener las cuentas.");
                Console.WriteLine(jsonString);
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private static async Task ConsultarMovimientosAsync()
        {
            Console.Clear();
            MostrarTitulo();
            Console.WriteLine("\nConsultar Movimientos");
            Console.WriteLine("=====================\n");

            Console.Write("Número de cuenta: ");
            string numeroCuenta = Console.ReadLine();

            string url = "Movimientos";
            if (!string.IsNullOrEmpty(numeroCuenta))
                url += "?numeroCuenta=" + Uri.EscapeDataString(numeroCuenta);

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Error al obtener los movimientos: " + response.ReasonPhrase);
                Console.ReadKey(); return;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<Movimiento>>>(jsonString);

            if (apiResponse != null && apiResponse.Mensaje == "ok" && apiResponse.Response != null)
            {
                Console.WriteLine("\nMovimientos encontrados:");
                Console.WriteLine("==============================================================");
                Console.WriteLine(" ID | Fecha       | Tipo        | Valor     | Saldo Final");
                Console.WriteLine("==============================================================");
                foreach (var m in apiResponse.Response)
                    Console.WriteLine(" " + m.MovId.ToString().PadRight(2) + " | " +
                                      m.MovFecha.ToString("yyyy-MM-dd") + " | " +
                                      (m.MovTipo ?? "").PadRight(10) + " | " +
                                      m.MovValor.ToString("C2").PadLeft(8) + " | " +
                                      m.MovSaldoFinal.ToString("C2").PadLeft(10));
                Console.WriteLine("==============================================================");
            }
            else
            {
                Console.WriteLine(apiResponse != null ? apiResponse.Mensaje : "Error al obtener los movimientos.");
                Console.WriteLine(jsonString);
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private static async Task RealizarDepositoAsync()
        {
            Console.Clear();
            MostrarTitulo();
            Console.WriteLine("\nRealizar Depósito");
            Console.WriteLine("=================\n");

            Console.Write("Número de cuenta: ");
            string numeroCuenta = Console.ReadLine();
            Console.Write("Valor: ");
            decimal valor;
            if (!decimal.TryParse(Console.ReadLine(), out valor) || valor <= 0)
            {
                Console.WriteLine("Ingrese un valor válido.");
                Console.ReadKey(); return;
            }

            string url = "Movimientos/Deposito?numeroCuenta=" + Uri.EscapeDataString(numeroCuenta) +
                         "&valor=" + valor.ToString(System.Globalization.CultureInfo.InvariantCulture);

            var response = await _httpClient.PostAsync(url, null);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Error al realizar el depósito: " + response.ReasonPhrase);
                Console.WriteLine(body);
                Console.ReadKey(); return;
            }

            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(body);
            if (apiResponse != null && (apiResponse.Mensaje == "ok" ||
                (apiResponse.Mensaje != null && apiResponse.Mensaje.IndexOf("depós", StringComparison.OrdinalIgnoreCase) >= 0)))
            {
                Console.WriteLine("\n==================================================");
                Console.WriteLine("|  Depósito realizado con éxito.                 |");
                Console.WriteLine("==================================================");
            }
            else
            {
                Console.WriteLine(apiResponse != null ? apiResponse.Mensaje : "No se pudo confirmar el depósito.");
                Console.WriteLine(body);
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private static async Task RealizarRetiroAsync()
        {
            Console.Clear();
            MostrarTitulo();
            Console.WriteLine("\nRealizar Retiro");
            Console.WriteLine("===============\n");

            Console.Write("Número de cuenta: ");
            string numeroCuenta = Console.ReadLine();
            Console.Write("Valor: ");
            decimal valor;
            if (!decimal.TryParse(Console.ReadLine(), out valor) || valor <= 0)
            {
                Console.WriteLine("Ingrese un valor válido.");
                Console.ReadKey(); return;
            }

            string url = "Movimientos/Retiro?numeroCuenta=" + Uri.EscapeDataString(numeroCuenta) +
                         "&valor=" + valor.ToString(System.Globalization.CultureInfo.InvariantCulture);

            var response = await _httpClient.PostAsync(url, null);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Error al realizar el retiro: " + response.ReasonPhrase);
                Console.WriteLine(body);
                Console.ReadKey(); return;
            }

            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(body);
            if (apiResponse != null && (apiResponse.Mensaje == "ok" ||
                (apiResponse.Mensaje != null && apiResponse.Mensaje.IndexOf("retiro", StringComparison.OrdinalIgnoreCase) >= 0)))
            {
                Console.WriteLine("\n==================================================");
                Console.WriteLine("|  Retiro realizado con éxito.                   |");
                Console.WriteLine("==================================================");
            }
            else
            {
                Console.WriteLine(apiResponse != null ? apiResponse.Mensaje : "No se pudo confirmar el retiro.");
                Console.WriteLine(body);
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private static async Task RealizarTransferenciaAsync()
        {
            Console.Clear();
            MostrarTitulo();
            Console.WriteLine("\nRealizar Transferencia");
            Console.WriteLine("======================\n");

            Console.Write("Cuenta Origen: ");
            string cuentaOrigen = Console.ReadLine();
            Console.Write("Cuenta Destino: ");
            string cuentaDestino = Console.ReadLine();
            Console.Write("Valor: ");
            decimal valor;
            if (!decimal.TryParse(Console.ReadLine(), out valor) || valor <= 0)
            {
                Console.WriteLine("Ingrese un valor válido.");
                Console.ReadKey(); return;
            }

            // ===== OPCIÓN A: enviar como form-url-encoded (recomendado) =====
            try
            {
                var pairs = new[]
                {
                    new KeyValuePair<string,string>("cuentaOrigen", cuentaOrigen),
                    new KeyValuePair<string,string>("cuentaDestino", cuentaDestino),
                    new KeyValuePair<string,string>("valor", valor.ToString(System.Globalization.CultureInfo.InvariantCulture))
                };

                using (var form = new System.Net.Http.FormUrlEncodedContent(pairs))
                {
                    var resp = await _httpClient.PostAsync("Transferencia", form);
                    var body = await resp.Content.ReadAsStringAsync();

                    if (resp.IsSuccessStatusCode)
                    {
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(body);
                        if (apiResponse != null && (apiResponse.Mensaje == "ok" ||
                            (apiResponse.Mensaje != null && apiResponse.Mensaje.IndexOf("transfer", StringComparison.OrdinalIgnoreCase) >= 0)))
                        {
                            Console.WriteLine("\n==================================================");
                            Console.WriteLine("|  Transferencia realizada con éxito.            |");
                            Console.WriteLine("==================================================");
                        }
                        else
                        {
                            Console.WriteLine(apiResponse != null ? apiResponse.Mensaje : "No se pudo confirmar la transferencia.");
                            Console.WriteLine(body);
                        }
                        Console.WriteLine("\nPresione cualquier tecla para continuar...");
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("[Transferencia FORM] HTTP " + (int)resp.StatusCode + " " + resp.ReasonPhrase);
                        if (!string.IsNullOrWhiteSpace(body))
                        {
                            Console.WriteLine("[Transferencia FORM] Cuerpo devuelto:");
                            Console.WriteLine(body);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Transferencia FORM] Excepción: " + ex.Message);
            }

            // ===== OPCIÓN B: enviar en el querystring (si tu API también lo acepta) =====
            try
            {
                string url = "Transferencia?cuentaOrigen=" + Uri.EscapeDataString(cuentaOrigen) +
                             "&cuentaDestino=" + Uri.EscapeDataString(cuentaDestino) +
                             "&valor=" + valor.ToString(System.Globalization.CultureInfo.InvariantCulture);

                var response = await _httpClient.PostAsync(url, null);
                var body2 = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(body2);
                    if (apiResponse != null && (apiResponse.Mensaje == "ok" ||
                        (apiResponse.Mensaje != null && apiResponse.Mensaje.IndexOf("transfer", StringComparison.OrdinalIgnoreCase) >= 0)))
                    {
                        Console.WriteLine("\n==================================================");
                        Console.WriteLine("|  Transferencia realizada con éxito.            |");
                        Console.WriteLine("==================================================");
                    }
                    else
                    {
                        Console.WriteLine(apiResponse != null ? apiResponse.Mensaje : "No se pudo confirmar la transferencia.");
                        Console.WriteLine(body2);
                    }
                }
                else
                {
                    Console.WriteLine("[Transferencia QUERY] HTTP " + (int)response.StatusCode + " " + response.ReasonPhrase);
                    if (!string.IsNullOrWhiteSpace(body2))
                    {
                        Console.WriteLine("[Transferencia QUERY] Cuerpo devuelto:");
                        Console.WriteLine(body2);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Transferencia QUERY] Excepción: " + ex.Message);
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}
