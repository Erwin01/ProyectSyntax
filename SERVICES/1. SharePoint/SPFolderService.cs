namespace SERVICES
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Models;
    using Newtonsoft.Json;

    public class SPFolderService : SPBaseService
    {
        public SPDefaultData<SPFolder> Create(string subSiteUrl, string serverRelativeUrl, string folderName, string query = "" )
        {
            try
            {
                var folderInfo = new  SPFolder
                {
                    MetadataType = new SPDefaultMetadata { ObjectType = "SP.Folder" },
                    ServerRelativeUrl = String.Format(Constants.FORMATS.GENERAL.URL_SEPARATOR, serverRelativeUrl, folderName)
                };
                var json = JsonConvert.SerializeObject(folderInfo);
                var content = new StringContent(json.ToString());
                content.Headers.ContentType = MediaTypeHeaderValue.Parse(Constants.MEDIATYPES.JSON_ODATA_VERBOSE);
                return Utils.CallRESTService<SPDefaultData<SPFolder>>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPFOLDER.GENERAL, this.Config[Constants.APPSETTINGS.SITE_URL] + subSiteUrl, query),
                    method: HttpRestMethod.POST,
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    headers: new Dictionary<string, string>
                    {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE},
                        {Constants.HEADERS.X_REQUEST_DIGEST,this.SPContext.FormDigestValue },
                    },
                    data: content
                );
            }
            catch (Exception)
            {
                throw;

            }

        }
        public dynamic BreakRoleInheritance(string subSiteUrl, Guid folderId, bool clearPermissions = true)
        {
            try
            {
                return Utils.CallRESTService<dynamic>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPFOLDER.BREAK_ROLE_INHERITANCE_BY_ID, this.Config[Constants.APPSETTINGS.SITE_URL] + subSiteUrl, folderId, (!clearPermissions).ToString().ToLowerInvariant()),
                    method: HttpRestMethod.POST,
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    headers: new Dictionary<string, string>
                    {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE},
                        {Constants.HEADERS.X_REQUEST_DIGEST,this.SPContext.FormDigestValue },
                    },
                    data: null
                );
            }
            catch (Exception)
            {
                throw;

            }
        }
        public SPDefaultData<SPDefaultResults<SPRoleAssigment>> GetRoleAssigments(string subSiteUrl, string serverRelativeUrl)
        {
            try
            {
                return Utils.CallRESTService<SPDefaultData<SPDefaultResults<SPRoleAssigment>>>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPFOLDER.ROLE_ASSIGMENTS, this.Config[Constants.APPSETTINGS.SITE_URL] + subSiteUrl, serverRelativeUrl),
                    method: HttpRestMethod.GET,
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    headers: new Dictionary<string, string>
                    {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE}
                    },
                    data: null
                );
            }
            catch (Exception)
            {
                throw;

            }
        }
        public dynamic RemoveRoleAssigment(string subSiteUrl, string serverRelativeUrl, int principalId)
        {
            try
            {
                return Utils.CallRESTService<SPDefaultData<SPDefaultResults<SPRoleAssigment>>>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPFOLDER.GET_ROLE_ASSIGMENT, this.Config[Constants.APPSETTINGS.SITE_URL] + subSiteUrl, serverRelativeUrl, principalId),
                    method: HttpRestMethod.DELETE,
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    headers: new Dictionary<string, string>
                    {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE} 
                    },
                    data: null
                );
            }
            catch (Exception)
            {
                throw;

            }
        }
        public dynamic AddRoleAssigment(string subSiteUrl, string serverRelativeUrl, int principalId, string roleDef)
        {
            try
            {
                return Utils.CallRESTService<SPDefaultData<SPDefaultResults<SPRoleAssigment>>>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPFOLDER.ADD_ROLE_ASSIGMENT, this.Config[Constants.APPSETTINGS.SITE_URL] + subSiteUrl, serverRelativeUrl, principalId, roleDef),
                    method: HttpRestMethod.POST,
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    headers: new Dictionary<string, string>
                    {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE}
                    },
                    data: null
                );
            }
            catch (Exception)
            {
                throw;

            }
        }
        public dynamic AddRoleAssigment(string subSiteUrl, Guid uniqueId, int principalId, string roleDef)
        {

            try
            {
                return Utils.CallRESTService<SPDefaultData<SPDefaultResults<SPRoleAssigment>>>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPFOLDER.ADD_ROLE_ASSIGMENT_BY_ID, this.Config[Constants.APPSETTINGS.SITE_URL] + subSiteUrl, uniqueId, principalId, roleDef),
                    method: HttpRestMethod.POST,
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    headers: new Dictionary<string, string>
                    {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE}
                    },
                    data: null
                );
            }
            catch (Exception)
            {
                throw;

            }
        }
        
    }
}
