namespace SERVICES.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json;
    using System.Text.Json.Serialization;

    public class OAuth2Token
    {
        [JsonProperty(Constants.PROPERTIES.TOKEN_TYPE)]
        [JsonPropertyName(Constants.PROPERTIES.TOKEN_TYPE)]
        public string TokenType { get; set; }
        [JsonProperty(Constants.PROPERTIES.EXPIRES_IN)]
        [JsonPropertyName(Constants.PROPERTIES.EXPIRES_IN)]
        public string ExpiresIn { get; set; }
        [JsonProperty(Constants.PROPERTIES.NOT_BEFORE)]
        [JsonPropertyName(Constants.PROPERTIES.NOT_BEFORE)]
        public string NotBefore { get; set; }
        [JsonProperty(Constants.PROPERTIES.EXPIRES_ON)]
        [JsonPropertyName(Constants.PROPERTIES.EXPIRES_ON)]
        public string ExpiresOn { get; set; }
        [JsonProperty(Constants.PROPERTIES.RESOURCE)]
        [JsonPropertyName(Constants.PROPERTIES.RESOURCE)]
        public string Resource { get; set; }
        [JsonProperty(Constants.PROPERTIES.ACCESS_TOKEN)]
        [JsonPropertyName(Constants.PROPERTIES.ACCESS_TOKEN)]
        public string AccessToken { get; set; }
    }
}