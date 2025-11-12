using System.ComponentModel.DataAnnotations;

namespace Eurekabank_Dotnet_RestFul_G6.ec.edu.monster.modelo
{
    public class Usuario
    {
        public int UsuId { get; set; }
        public string UsuNombre { get; set; }
        public string UsuApellido { get; set; }
        public string UsuCorreo { get; set; }
        public string UsuTelefono { get; set; }
        public string UsuCedula { get; set; }
        public string UsuRol { get; set; }
        public string UsuContrasena { get; set; }
    }
}
