namespace EurekabankClienteConsola.Models
{
    public class Deposito
    {
        // Número de cuenta a la que se hará el depósito
        public string NumeroCuenta { get; set; }

        // Monto del depósito
        public decimal Valor { get; set; }

        // (Opcional) Mensaje que puede devolver el backend
        public string Mensaje { get; set; }

        // (Opcional) Nuevo saldo después del depósito
        public decimal? NuevoSaldo { get; set; }
    }
}
