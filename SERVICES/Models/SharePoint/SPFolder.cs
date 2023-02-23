using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERVICES.Models
{
    public class SPFolder: SPDefaultObject
    {
        public string ServerRelativeUrl { get; set; }
        public Guid UniqueId { get; set; }
    }
}
