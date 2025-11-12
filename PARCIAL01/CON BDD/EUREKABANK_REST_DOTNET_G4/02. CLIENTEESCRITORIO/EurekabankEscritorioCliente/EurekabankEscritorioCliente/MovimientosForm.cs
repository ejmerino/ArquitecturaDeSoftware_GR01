using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using EurekabankEscritorioCliente.Models;

namespace EurekabankEscritorioCliente
{
    public partial class MovimientosForm : Form
    {
        private HttpClient _httpClient;

        public MovimientosForm(HttpClient httpClient)
        {
            InitializeComponent();
            _httpClient = httpClient;
        }

        private async void btnBuscar_Click(object sender, EventArgs e)
        {
            string numeroCuenta = txtNumeroCuenta.Text;
            await BuscarMovimientosAsync(numeroCuenta);
        }

        private async Task BuscarMovimientosAsync(string numeroCuenta)
        {
            try
            {
                // Construir la URL con el parámetro de búsqueda
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
                        dgvMovimientos.DataSource = apiResponse.Response;
                    }
                    else
                    {
                        MessageBox.Show("Error al obtener los movimientos.");
                    }
                }
                else
                {
                    MessageBox.Show("Error al obtener los movimientos: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se produjo un error al realizar la solicitud: " + ex.Message);
            }
        }
    }
}
