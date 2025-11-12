namespace EurekabankClienteConsola.Models
{
    public class ApiResponse<T>
    {
        public string Mensaje { get; set; }
        public T Response { get; set; }
    }

}