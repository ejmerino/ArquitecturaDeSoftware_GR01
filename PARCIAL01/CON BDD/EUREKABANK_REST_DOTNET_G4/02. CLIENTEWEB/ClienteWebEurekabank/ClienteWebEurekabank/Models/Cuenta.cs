using System.ComponentModel.DataAnnotations;

namespace ClienteWebEurekabank.Models
{
    public class Cuenta
    {
        [Key]
        public int CuId { get; set; }

        public int CliId { get; set; }

        public string CuNumero { get; set; }

        public string CuTipo { get; set; }

        public float? CuSaldo { get; set; }
    }
}
