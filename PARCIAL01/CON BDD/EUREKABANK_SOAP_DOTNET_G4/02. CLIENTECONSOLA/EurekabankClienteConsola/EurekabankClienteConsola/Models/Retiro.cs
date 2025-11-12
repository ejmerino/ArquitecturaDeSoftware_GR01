namespace EurekabankClienteConsola.Models
{
    public class Retiro
    {
        // Número de cuenta de la que se hará el retiro
        public string NumeroCuenta { get; set; }

        // Monto a retirar
        public decimal Valor { get; set; }

        // (Opcional) Mensaje del backend
        public string Mensaje { get; set; }

        // (Opcional) Nuevo saldo después del retiro
        public decimal? NuevoSaldo { get; set; }
    }
}
