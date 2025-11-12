using System;

namespace EurekabankEscritorioCliente.Models
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
