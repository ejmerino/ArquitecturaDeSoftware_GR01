using System.ComponentModel.DataAnnotations;

namespace ClienteWebEurekabank.Models
{
    public class TransferenciaModel
    {
        public string CuentaOrigen { get; set; }
        public string CuentaDestino { get; set; }
        public float Valor { get; set; }
    }
}

