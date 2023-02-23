namespace SERVICES
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using Models;
    public class SPWebService: SPBaseService
    {
        
        public SPWebService():base()
        {
            
        }

        public SPDefaultData<SPWeb> CreateWeb(SPWebInformation webInfo, string subsite = "")
        {
            var json = JsonConvert.SerializeObject(webInfo);
            var content = new StringContent(json.ToString());
            content.Headers.ContentType = MediaTypeHeaderValue.Parse(Constants.MEDIATYPES.JSON_ODATA_VERBOSE);

            var spWebInfo = Utils.CallRESTService<SPDefaultData<SPWeb>>(
                url: string.Format(Constants.FORMATS.SPROUTES.SPWEB.ADD,this.Config[Constants.APPSETTINGS.SITE_URL] + subsite),
                method:HttpRestMethod.POST,
                authorization: Constants.AUTHORIZATION.BEARER,
                token: this.Token.AccessToken,
                headers: new Dictionary<string, string>
                {
                    {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE},
                    {Constants.HEADERS.X_REQUEST_DIGEST,this.SPContext.FormDigestValue }

                },
                data: content
                );
            return spWebInfo;
            
        }
        public bool DeleteWeb(string subSiteUrl)
        {

            try
            {
                var url = String.Format(Constants.FORMATS.GENERAL.URL_SEPARATOR, this.Config[Constants.APPSETTINGS.SITE_URL], subSiteUrl);
                Utils.CallRESTService<JToken>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPWEB.GENERAL, url),
                    method: HttpRestMethod.POST,
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    headers: new Dictionary<string, string>
                    {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE},
                        {Constants.HEADERS.X_REQUEST_DIGEST,this.SPContext.FormDigestValue },
                        {Constants.HEADERS.X_HTTP_METHOD, HttpRestMethod.DELETE.ToString() }
                    }
                );
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool EditWeb(string subSiteUrl,SPWebEditInfo webInfo)
        {
            var json = JsonConvert.SerializeObject(webInfo);
            var content = new StringContent(json.ToString());
            content.Headers.ContentType = MediaTypeHeaderValue.Parse(Constants.MEDIATYPES.JSON_ODATA_VERBOSE);
            var url = String.Format(Constants.FORMATS.GENERAL.URL_SEPARATOR, this.Config[Constants.APPSETTINGS.SITE_URL], subSiteUrl);


            Utils.CallRESTService<SPDefaultData<SPWeb>>(
                url: string.Format(Constants.FORMATS.SPROUTES.SPWEB.GENERAL, url),
                method: HttpRestMethod.POST,
                authorization: Constants.AUTHORIZATION.BEARER,
                token: this.Token.AccessToken,
                headers: new Dictionary<string, string>
                {
                    {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE},
                    {Constants.HEADERS.X_REQUEST_DIGEST,this.SPContext.FormDigestValue },
                    {Constants.HEADERS.X_HTTP_METHOD, HttpRestMethod.PATCH.ToString() },
                    {Constants.HEADERS.IF_MATCH, "*" }

                },
                data: content
                );
            return true;
        }
        public bool AddRoleAssigment(string idGroup, string roleDefId, string subSiteUrl = "") 
        {
            var url = String.Format(Constants.FORMATS.GENERAL.URL_SEPARATOR, this.Config[Constants.APPSETTINGS.SITE_URL], subSiteUrl);
            Utils.CallRESTService<JToken>(
                url: string.Format(Constants.FORMATS.SPROUTES.SPWEB.ADD_ROLE_ASSIGMENT, url, idGroup, roleDefId),
                method: HttpRestMethod.POST,
                authorization: Constants.AUTHORIZATION.BEARER,
                token: this.Token.AccessToken,
                headers: new Dictionary<string, string>
                {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE},
                        {Constants.HEADERS.X_REQUEST_DIGEST,this.SPContext.FormDigestValue },
                        {Constants.HEADERS.X_HTTP_METHOD, HttpRestMethod.POST.ToString() }
                }
            );
            return true;
        }
        public SPDefaultData<SPUser> EnsureUser(string email, string subSiteUrl = "")
        {
            var url = String.Format(Constants.FORMATS.GENERAL.URL_SEPARATOR, this.Config[Constants.APPSETTINGS.SITE_URL], subSiteUrl);
            return Utils.CallRESTService<SPDefaultData<SPUser>>(
                url: string.Format(Constants.FORMATS.SPROUTES.SPWEB.ENSURE_USER, url, email),
                method: HttpRestMethod.POST,
                authorization: Constants.AUTHORIZATION.BEARER,
                token: this.Token.AccessToken,
                headers: new Dictionary<string, string>
                {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE},
                        {Constants.HEADERS.X_REQUEST_DIGEST,this.SPContext.FormDigestValue },
                }
            );
        }

    }
}
