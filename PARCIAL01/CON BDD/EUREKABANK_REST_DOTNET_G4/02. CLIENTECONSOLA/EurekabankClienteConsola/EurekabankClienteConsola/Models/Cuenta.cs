using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EurekabankClienteConsola.Models
{
    public class Cuenta
    {
        public int CuId { get; set; }
        public int CliId { get; set; }
        public string CuNumero { get; set; }
        public string CuTipo { get; set; }
        public float? CuSaldo { get; set; }
    }
}
