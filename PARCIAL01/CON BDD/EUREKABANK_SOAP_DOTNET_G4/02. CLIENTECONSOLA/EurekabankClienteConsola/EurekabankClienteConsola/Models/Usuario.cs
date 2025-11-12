using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EurekabankClienteConsola.Models
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
