using System;
using System.Net.Http;
using System.Windows.Forms;

namespace EurekabankEscritorioCliente
{
    public partial class MenuForm : Form
    {
        private string usuarioNombre;
        private readonly HttpClient _httpClient;

        public MenuForm(string usuarioNombre, HttpClient httpClient)
        {
            InitializeComponent();
            this.usuarioNombre = usuarioNombre;
            _httpClient = httpClient;
            lblBienvenido.Text = $"Bienvenido, {usuarioNombre}";
        }

        private void btnCuentas_Click(object sender, EventArgs e)
        {
            CuentasForm cuentasForm = new CuentasForm(_httpClient);
            cuentasForm.Show();
        }

        private void btnMovimientos_Click(object sender, EventArgs e)
        {
            MovimientosForm movimientosForm = new MovimientosForm(_httpClient);
            movimientosForm.Show();
        }

        private void btnTransferencias_Click(object sender, EventArgs e)
        {
            TransferenciasForm transferenciasForm = new TransferenciasForm(_httpClient);
            transferenciasForm.Show();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            // Cierra el formulario actual y regresa al formulario de login
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }
    }
}
