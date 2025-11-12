using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using EurekabankEscritorioCliente.Models;
using Newtonsoft.Json;

namespace EurekabankEscritorioCliente
{
    public partial class TransferenciasForm : Form
    {
        private HttpClient _httpClient;

        public TransferenciasForm(HttpClient httpClient)
        {
            InitializeComponent();
            _httpClient = httpClient;
        }

        private async void btnRealizarTransferencia_Click(object sender, EventArgs e)
        {
            string cuentaOrigen = txtCuentaOrigen.Text;
            string cuentaDestino = txtCuentaDestino.Text;
            if (float.TryParse(txtValor.Text, out float valor))
            {
                await RealizarTransferenciaAsync(cuentaOrigen, cuentaDestino, valor);
            }
            else
            {
                MessageBox.Show("Ingrese un valor válido.");
            }
        }

        private async Task RealizarTransferenciaAsync(string cuentaOrigen, string cuentaDestino, float valor)
        {
            try
            {
                // Construir la URL con los parámetros de la transferencia
                string url = $"Transferencia?cuentaOrigen={cuentaOrigen}&cuentaDestino={cuentaDestino}&valor={valor}";

                HttpResponseMessage response = await _httpClient.PostAsync(url, null);

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(jsonString);

                    if (apiResponse.Mensaje == "Transferencia realizada con éxito.")
                    {
                        MessageBox.Show(apiResponse.Mensaje);
                    }
                    else
                    {
                        MessageBox.Show("Error al realizar la transferencia.");
                    }
                }
                else
                {
                    MessageBox.Show("Error al realizar la transferencia: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se produjo un error al realizar la solicitud: " + ex.Message);
            }
        }

        // Eventos para simular el PlaceholderText
        private void txtCuentaOrigen_Enter(object sender, EventArgs e)
        {
            if (txtCuentaOrigen.Text == "Cuenta Origen")
            {
                txtCuentaOrigen.Text = "";
                txtCuentaOrigen.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void txtCuentaOrigen_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCuentaOrigen.Text))
            {
                txtCuentaOrigen.Text = "Cuenta Origen";
                txtCuentaOrigen.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void txtCuentaDestino_Enter(object sender, EventArgs e)
        {
            if (txtCuentaDestino.Text == "Cuenta Destino")
            {
                txtCuentaDestino.Text = "";
                txtCuentaDestino.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void txtCuentaDestino_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCuentaDestino.Text))
            {
                txtCuentaDestino.Text = "Cuenta Destino";
                txtCuentaDestino.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void txtValor_Enter(object sender, EventArgs e)
        {
            if (txtValor.Text == "Valor")
            {
                txtValor.Text = "";
                txtValor.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void txtValor_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtValor.Text))
            {
                txtValor.Text = "Valor";
                txtValor.ForeColor = System.Drawing.Color.Gray;
            }
        }
    }
}
