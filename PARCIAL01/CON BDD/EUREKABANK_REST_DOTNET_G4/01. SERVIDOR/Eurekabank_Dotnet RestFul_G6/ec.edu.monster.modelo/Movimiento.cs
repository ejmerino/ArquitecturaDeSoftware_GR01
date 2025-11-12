using System.ComponentModel.DataAnnotations;

namespace Eurekabank_Dotnet_RestFul_G6.ec.edu.monster.modelo
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
