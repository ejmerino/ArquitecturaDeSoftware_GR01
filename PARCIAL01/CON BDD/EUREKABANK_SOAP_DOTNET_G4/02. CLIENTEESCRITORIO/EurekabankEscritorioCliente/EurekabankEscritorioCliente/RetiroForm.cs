using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using EurekabankEscritorioCliente.Models;
using System.Collections.Generic;

namespace EurekabankEscritorioCliente
{
    public partial class CuentasForm : Form
    {
        private HttpClient _httpClient;

        public CuentasForm(HttpClient httpClient)
        {
            InitializeComponent();
            _httpClient = httpClient;
        }

        private async void btnBuscar_Click(object sender, EventArgs e)
        {
            string numeroCuenta = txtNumeroCuenta.Text;
            await BuscarCuentasAsync(numeroCuenta);
        }

        private async Task BuscarCuentasAsync(string numeroCuenta)
        {
            try
            {
                // Construir la URL con el parámetro de búsqueda
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
                        dgvCuentas.DataSource = apiResponse.Response;
                    }
                    else
                    {
                        MessageBox.Show("Error al obtener las cuentas.");
                    }
                }
                else
                {
                    MessageBox.Show("Error al obtener las cuentas: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se produjo un error al realizar la solicitud: " + ex.Message);
            }
        }
    }
}
