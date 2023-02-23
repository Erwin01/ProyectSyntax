namespace SERVICES
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using ViewModels = Models;
    public class OAuth2Service: MainService
    {
        public OAuth2Service():base()
        {
        }
        public ViewModels.OAuth2Token GetAuth2TokenSP()
        {
            var jsonToSend = JsonConvert.SerializeObject(new ViewModels.OAut2Request
            {
                grant_type = this.Config[Constants.APPSETTINGS.GRANT_TYPE],
                client_id = string.Format(Constants.FORMATS.OAUTH2.CLIENT_ID_SP, 
                                          this.Config[Constants.APPSETTINGS.CLIENT_ID], 
                                          this.Config[Constants.APPSETTINGS.TENANT_ID]),
                client_secret = this.Config[Constants.APPSETTINGS.CLIENT_SECRET],
                resource = string.Format(Constants.FORMATS.OAUTH2.RESOURCE_SP, 
                                         this.Config[Constants.APPSETTINGS.RESOURCE_ID], 
                                         this.Config[Constants.APPSETTINGS.DOMAIN], 
                                         this.Config[Constants.APPSETTINGS.TENANT_ID])

            }, Formatting.None);
            return Utils.CallRESTService<ViewModels.OAuth2Token>(
                url: string.Format(Constants.FORMATS.OAUTH2.OAUTH_URL_SP, this.Config[Constants.APPSETTINGS.TENANT_ID]),
                method: HttpRestMethod.POST,
                data: new FormUrlEncodedContent(JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonToSend)));
        }
    }
}
