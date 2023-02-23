using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERVICES.Models
{
    public class SPBatchDefinition<T>
    {
        public HttpRestMethod? Method { get; set; }
        public string EndPoint { get; set; }
        public Guid ChangeSetId { get; set; }
        public T Item { get; set; }
        public bool IsLastItem { get; set; }
        public Dictionary<string,string> Headers { get; set; }

        public SPBatchDefinition()
        {
            IsLastItem = false;
            Headers = new Dictionary<string, string>();
        }
    }
}
