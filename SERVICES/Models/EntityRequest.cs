namespace SERVICES
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Text;
    public class EntityRequest
    {
        [JsonProperty("parentEntityLogicalName")]
        public string ParentEntityLogicalName { get; set; }
        [JsonProperty("parentEntityField")]
        public string ParentEntityField { get; set; }
    }
}
