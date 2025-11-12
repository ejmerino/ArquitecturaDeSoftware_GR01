using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EurekabankClienteConsola.Models
{
    public class ResponseModel<T>
    {
        public string Mensaje { get; set; }
        public List<T> Response { get; set; }
    }
}
