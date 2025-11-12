using System.Collections.Generic;

namespace ClienteWebEurekabank.Models
{
    public class ResponseModel<T>
    {
        public string Mensaje { get; set; }
        public List<T> Response { get; set; }
    }
}
