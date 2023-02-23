using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SERVICES.Models
{
    public class SPDefaultData<T>
    {
        [JsonProperty(Constants.PROPERTIES.SP_D)]
        [JsonPropertyName(Constants.PROPERTIES.SP_D)]
        public T Data { get; set; }
    }

    public class SPDefaultResults<T> : SPDefaultObject
    {
        [JsonProperty(Constants.PROPERTIES.SP_RESULTS)]
        [JsonPropertyName(Constants.PROPERTIES.SP_RESULTS)]
        public ICollection<T> Results { get; set; }
    }
    public class SPDefaultObject
    {
        [JsonProperty(Constants.PROPERTIES.SP_METADATA)]
        [JsonPropertyName(Constants.PROPERTIES.SP_METADATA)]
        public virtual SPDefaultMetadata MetadataType { get; set; }
    }

    public class SPDefaultMetadata
    {
        [JsonProperty(Constants.PROPERTIES.SP_TYPE)]
        [JsonPropertyName(Constants.PROPERTIES.SP_TYPE)]
        public string ObjectType { get; set; }
    }
}
