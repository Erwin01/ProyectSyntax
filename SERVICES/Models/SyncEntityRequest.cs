using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERVICES.Models
{
    public class SyncEntityRequest
    {
        [JsonProperty( PropertyName = "initDate")]
        public DateTime InitDate { get; set; }
        [JsonProperty(PropertyName = "endDate")]
        public DateTime EndDate { get; set; }
    }
}
