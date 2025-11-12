using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using EurekabankEscritorioCliente.Models;

namespace EurekabankEscritorioCliente
{
    public partial class LoginForm : MaterialForm
    {
        public LoginForm()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue600, Primary.Blue700, Primary.Blue500, Accent.LightBlue200, TextShade.WHITE);
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string correo = txtCorreo.Text;
            string contrasena = txtContrasena.Text;

            Usuario usuario = await AutenticarUsuario(correo, contrasena);

            if (usuario != null)
            {
                // Asegúrate de pasar el HttpClient a MenuForm
                HttpClient httpClient = new HttpClient
                {
                    BaseAddress = new Uri("https://localhost:5278/api/EurekabankControlador/")
                };

                MenuForm menuForm = new MenuForm(usuario.UsuNombre, httpClient);
                menuForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Credenciales inválidas.");
            }
        }

        private async Task<Usuario> AutenticarUsuario(string correo, string contrasena)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:5278/api/EurekabankControlador/");

                string url = $"Usuarios/PorCorreo?correo={correo}&contrasena={contrasena}";

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    ApiResponse<Usuario> apiResponse = await response.Content.ReadAsAsync<ApiResponse<Usuario>>();
                    if (apiResponse.Mensaje == "ok")
                    {
                        return apiResponse.Response; // Devuelve el usuario si la autenticación es exitosa
                    }
                }

                return null; // Devuelve null si la autenticación falla
            }
        }
    }
}
