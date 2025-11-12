namespace ClienteWebEurekabank.Models
{
    public class OperacionModel
    {
        public string NumeroCuenta { get; set; }
        public decimal Valor { get; set; }
        public string Mensaje { get; set; } // opcional para mostrar resultado
    }
}
