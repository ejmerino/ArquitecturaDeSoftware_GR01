using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EurekabankClienteConsola.Models
{
    public class Movimiento
    {
        public int MovId { get; set; }
        public int CuId { get; set; }
        public DateTime MovFecha { get; set; }
        public string MovTipo { get; set; }
        public float MovValor { get; set; }
        public float MovSaldoFinal { get; set; }
    }
}
