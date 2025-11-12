using System;
using System.ComponentModel.DataAnnotations;

namespace ClienteWebEurekabank.Models
{
    public class Movimiento
    {
        [Key]
        public int MovId { get; set; }

        public int CuId { get; set; }

        public DateTime? MovFecha { get; set; }

        public string MovTipo { get; set; }

        public float? MovValor { get; set; }

        public float? MovSaldoFinal { get; set; }
    }
}
