namespace SERVICES.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.Json.Serialization;

    public class SPWebEditInfo: SPDefaultObject
    {
        public string Description { get; set; }
        public string Title { get; set; }
    }
    public class SPWebBaseInfo: SPWebEditInfo
    {
        public string Url { get; set; }
        public int Language { get; set; }
        public string WebTemplate { get; set; }
        public bool UseUniquePermissions { get; set; }
    }
    public class SPWeb: SPWebBaseInfo
    {
        public int Configuration { get; set; }
        public DateTime Created { get; set; }
        public string Id { get; set; }
        public DateTime LastItemModifiedDate { get; set; }
        public DateTime LastItemUserModifiedDate { get; set; }
        public int WebTemplateId { get; set; }
        public string ServerRelativeUrl { get; set; }
    }
    public class SPWebContext : SPDefaultObject
    {
        public SPContextInfo GetContextWebInformation { get; set; }
    }
    public class SPWebInformation
    {
        [JsonProperty("parameters")]
        [JsonPropertyName("parameters")]
        public SPWebBaseInfo Parameters { get; set; }
    }

}
