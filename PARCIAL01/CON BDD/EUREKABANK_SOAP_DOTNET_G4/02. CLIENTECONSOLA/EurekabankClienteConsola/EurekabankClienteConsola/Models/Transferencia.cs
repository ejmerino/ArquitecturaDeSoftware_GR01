using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EurekabankClienteConsola.Models
{
    public class Transferencia
    {
        public string CuentaOrigen { get; set; }
        public string CuentaDestino { get; set; }
        public float Valor { get; set; }
    }

}
