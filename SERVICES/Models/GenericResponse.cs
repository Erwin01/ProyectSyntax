namespace SERVICES.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class GenericResponse
    {
        public string status { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public int? Count { get; set; }
        public object result { get; set; }
    }
}
