namespace EurekabankEscritorioCliente.Models
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
