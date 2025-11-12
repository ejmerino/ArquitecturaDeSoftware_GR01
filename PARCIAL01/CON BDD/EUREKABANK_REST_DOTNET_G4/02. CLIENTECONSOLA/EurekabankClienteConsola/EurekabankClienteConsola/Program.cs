using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EurekabankClienteConsola.Models;

namespace EurekabankClienteConsola
{
    class Program
    {
        private static HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:5278/api/EurekabankControlador/") };

        static async Task Main(string[] args)
        {
            Console.WriteLine(@"
==================================================
|                                                |
|       Bienvenido al Sistema Eurekabank         |
|                                                |
==================================================");

            Usuario usuario = await LoginAsync();

            if (usuario != null)
            {
                await MostrarMenuAsync(usuario);
            }
            else
            {
                Console.WriteLine("Credenciales inválidas. Saliendo...");
            }
        }

        private static async Task<Usuario> LoginAsync()
        {
            Console.WriteLine("\nPor favor, ingrese sus credenciales.");
            Console.Write("Correo: ");
            string correo = Console.ReadLine();
            Console.Write("Contraseña: ");
            string contrasena = Console.ReadLine();

            string url = $"Usuarios/PorCorreo?correo={correo}&contrasena={contrasena}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                ApiResponse<Usuario> apiResponse = await response.Content.ReadAsAsync<ApiResponse<Usuario>>();
                if (apiResponse.Mensaje == "ok")
                {
                    return apiResponse.Response;
                }
            }

            return null;
        }

        private static async Task MostrarMenuAsync(Usuario usuario)
        {
            bool continuar = true;
            while (continuar)
            {
                Console.Clear();
                Console.WriteLine($@"
==================================================
|                                                |
|       Bienvenido, {usuario.UsuNombre}          |
|                                                |
==================================================");
                Console.WriteLine("1. Consultar Cuentas");
                Console.WriteLine("2. Consultar Movimientos");
                Console.WriteLine("3. Realizar Transferencia");
                Console.WriteLine("4. Cerrar Sesión");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        await ConsultarCuentasAsync();
                        break;
                    case "2":
                        await ConsultarMovimientosAsync();
                        break;
                    case "3":
                        await RealizarTransferenciaAsync();
                        break;
                    case "4":
                        continuar = false;
                        break;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }
            }
        }

        private static async Task ConsultarCuentasAsync()
        {
            Console.Write("Número de cuenta (deje en blanco para todas): ");
            string numeroCuenta = Console.ReadLine();

            string url = "Cuentas";
            if (!string.IsNullOrEmpty(numeroCuenta))
            {
                url += $"?numeroCuenta={numeroCuenta}";
            }

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<Cuenta>>>(jsonString);

                if (apiResponse.Mensaje == "ok")
                {
                    Console.WriteLine("\nCuentas encontradas:");
                    Console.WriteLine("==================================================");
                    Console.WriteLine(" ID | Número de Cuenta | Tipo | Saldo          ");
                    Console.WriteLine("==================================================");
                    foreach (var cuenta in apiResponse.Response)
                    {
                        Console.WriteLine($" {cuenta.CuId,-2} | {cuenta.CuNumero,-16} | {cuenta.CuTipo,-4} | {cuenta.CuSaldo,-13} ");
                    }
                    Console.WriteLine("==================================================");
                }
                else
                {
                    Console.WriteLine("Error al obtener las cuentas.");
                }
            }
            else
            {
                Console.WriteLine("Error al obtener las cuentas: " + response.ReasonPhrase);
            }

            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private static async Task ConsultarMovimientosAsync()
        {
            Console.Write("Número de cuenta: ");
            string numeroCuenta = Console.ReadLine();

            string url = "Movimientos";
            if (!string.IsNullOrEmpty(numeroCuenta))
            {
                url += $"?numeroCuenta={numeroCuenta}";
            }

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<Movimiento>>>(jsonString);

                if (apiResponse.Mensaje == "ok")
                {
                    Console.WriteLine("\nMovimientos encontrados:");
                    Console.WriteLine("==============================================================");
                    Console.WriteLine(" ID | Fecha       | Tipo   | Valor  | Saldo Final           ");
                    Console.WriteLine("==============================================================");
                    foreach (var movimiento in apiResponse.Response)
                    {
                        Console.WriteLine($" {movimiento.MovId,-2} | {movimiento.MovFecha,-10} | {movimiento.MovTipo,-6} | {movimiento.MovValor,-6} | {movimiento.MovSaldoFinal,-21} ");
                    }
                    Console.WriteLine("==============================================================");
                }
                else
                {
                    Console.WriteLine("Error al obtener los movimientos.");
                }
            }
            else
            {
                Console.WriteLine("Error al obtener los movimientos: " + response.ReasonPhrase);
            }

            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private static async Task RealizarTransferenciaAsync()
        {
            Console.Write("Cuenta Origen: ");
            string cuentaOrigen = Console.ReadLine();
            Console.Write("Cuenta Destino: ");
            string cuentaDestino = Console.ReadLine();
            Console.Write("Valor: ");
            if (float.TryParse(Console.ReadLine(), out float valor))
            {
                string url = $"Transferencia?cuentaOrigen={cuentaOrigen}&cuentaDestino={cuentaDestino}&valor={valor}";

                HttpResponseMessage response = await _httpClient.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(jsonString);

                    if (apiResponse.Mensaje == "Transferencia realizada con éxito.")
                    {
                        Console.WriteLine("\n==================================================");
                        Console.WriteLine("|  Transferencia realizada con éxito.            |");
                        Console.WriteLine("==================================================");
                    }
                    else
                    {
                        Console.WriteLine("Error al realizar la transferencia.");
                    }
                }
                else
                {
                    Console.WriteLine("Error al realizar la transferencia: " + response.ReasonPhrase);
                }
            }
            else
            {
                Console.WriteLine("Ingrese un valor válido.");
            }

            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}
