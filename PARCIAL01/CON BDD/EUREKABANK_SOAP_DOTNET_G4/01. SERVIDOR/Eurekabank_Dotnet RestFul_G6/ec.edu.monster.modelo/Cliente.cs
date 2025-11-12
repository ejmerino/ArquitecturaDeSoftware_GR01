using System.ComponentModel.DataAnnotations;

namespace Eurekabank_Dotnet_RestFul_G6.ec.edu.monster.modelo
{
    public class Cliente
    {
        [Key]
        public int CliId { get; set; }

        public string CliCedula { get; set; }

        public string CliNombre { get; set; }

        public string CliApellido { get; set; }

        public string CliCorreo { get; set; }

        public string CliTelefono { get; set; }
    }
}
