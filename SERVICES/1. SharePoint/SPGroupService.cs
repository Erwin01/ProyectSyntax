namespace SERVICES
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Linq;
    using System.Text;
    using ViewModels = Models;
    using System.IO;
    using Microsoft.SharePoint.Client;
    using OfficeDevPnP.Core;

    public class SPGroupService: SPBaseService
    {
        public ClientContext _pnpClientContext;

        public SPGroupService() : base() { }

        public SPGroupService(string siteUrl)
        {
            AuthenticationManager authManager = new AuthenticationManager();
            _pnpClientContext  = authManager.GetAppOnlyAuthenticatedContext(siteUrl, Config[Constants.APPSETTINGS.CLIENT_ID], Config[Constants.APPSETTINGS.CLIENT_SECRET]);
        }
        public ViewModels.SPDefaultData<ViewModels.SPGroup> Create(ViewModels.SPGroupInfo groupInfo)
        {
            try
            {
                var json = JsonConvert.SerializeObject(groupInfo);
                var content = new StringContent(json.ToString());
                content.Headers.ContentType = MediaTypeHeaderValue.Parse(Constants.MEDIATYPES.JSON_ODATA_VERBOSE);

                return Utils.CallRESTService<ViewModels.SPDefaultData<ViewModels.SPGroup>>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPGROUP.GENERAL, this.Config[Constants.APPSETTINGS.SITE_URL]),
                    method: HttpRestMethod.POST,
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    headers: new Dictionary<string, string>
                    {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE},
                        {Constants.HEADERS.X_REQUEST_DIGEST,this.SPContext.FormDigestValue }
                    },
                    data: content
                );
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool Delete(string idGroup)
        {
            try
            {
                Utils.CallRESTService<JToken>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPGROUP.REMOVE_BY_ID, this.Config[Constants.APPSETTINGS.SITE_URL], idGroup),
                    method: HttpRestMethod.POST,
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    headers: new Dictionary<string, string>
                    {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE},
                        {Constants.HEADERS.X_REQUEST_DIGEST,this.SPContext.FormDigestValue }
                    }
                );
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ViewModels.SPDefaultData<ViewModels.SPDefaultResults<ViewModels.SPUser>> GetUsers(string idGroup)
        {
            try
            {
                return Utils.CallRESTService<ViewModels.SPDefaultData<ViewModels.SPDefaultResults<ViewModels.SPUser>>>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPGROUP.GET_USERS, this.Config[Constants.APPSETTINGS.SITE_URL], idGroup),
                    method: HttpRestMethod.GET,
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    headers: new Dictionary<string, string>
                    {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE},
                        {Constants.HEADERS.X_REQUEST_DIGEST,this.SPContext.FormDigestValue }
                    }
                );
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string AddUsersAsBatch(ICollection<ViewModels.SPUserInfoBatch> users)
        {
            try
            {
                var batchId = Guid.NewGuid();
                var changeSetId = Guid.NewGuid();
                var content = new MultipartContent("mixed", string.Format("batch_{0}", batchId));
                var batchItemDefs = users.Select((user, index) => new ViewModels.SPBatchDefinition<dynamic>
                {
                    ChangeSetId = changeSetId,
                    EndPoint = string.Format(Constants.FORMATS.SPROUTES.SPGROUP.GET_USERS, this.Config[Constants.APPSETTINGS.SITE_URL], user.IdGroup),
                    Method = HttpRestMethod.POST,
                    IsLastItem = index == users.Count() - 1 ,
                    Item = new ViewModels.SPUserInfo { LoginName = user.LoginName}
                }).ToList();
                var batchContentItem = CreateBatchContentItem<dynamic>(batchItemDefs);
                content.Add(batchContentItem);

                return Utils.CallRESTService<Stream>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPWEB.BATCH, this.Config[Constants.APPSETTINGS.SITE_URL]),
                    method: HttpRestMethod.POST,
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    headers: new Dictionary<string, string>
                    {
                    {Constants.HEADERS.X_REQUEST_DIGEST,this.SPContext.FormDigestValue }
                    },
                    data: content
                );
            }
            catch (Exception)
            {

                throw;
            }

        }
        public void DeleteUsersAsBatch(ICollection<ViewModels.SPUser> users, string idGroup)
        {
            try
            {
                using (_pnpClientContext)
                {
                    var group = _pnpClientContext.Web.SiteGroups.GetById(int.Parse(idGroup));
                    foreach(var user in users)
                    {
                        group.Users.RemoveById(user.Id);
                        _pnpClientContext.ExecuteQuery();
                    }
                }

            }
            catch (Exception ex)
            {

                throw (ex);
            }
        }
        public void DeleteAsBatch(ICollection<ViewModels.SPGroup> groups)
        {
            try
            {
                var batchId = Guid.NewGuid();
                var changeSetId = Guid.NewGuid();
                var content = new MultipartContent("mixed", string.Format("batch_{0}", batchId));
                var batchItemDefs = groups.Select((group, index) => new ViewModels.SPBatchDefinition<dynamic>
                {
                    ChangeSetId = changeSetId,
                    EndPoint = string.Format(Constants.FORMATS.SPROUTES.SPGROUP.REMOVE_BY_ID, this.Config[Constants.APPSETTINGS.SITE_URL], group.Id),
                    Method = HttpRestMethod.POST,
                    Item = new {},
                    IsLastItem = index == groups.Count() - 1
                }).ToList();
                var batchContentItem = CreateBatchContentItem<dynamic>(batchItemDefs);
                content.Add(batchContentItem);
                var response = Utils.CallRESTService<Stream>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPWEB.BATCH, this.Config[Constants.APPSETTINGS.SITE_URL]),
                    method: HttpRestMethod.POST,
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    headers: new Dictionary<string, string>
                    {
                        {Constants.HEADERS.X_REQUEST_DIGEST,this.SPContext.FormDigestValue }
                    },
                    data: content
                );

            }
            catch (Exception ex)
            {

                throw (ex);
            }
        }


        public void AddUserAsBatch(ViewModels.SPUser user, int idGroup){
            try
            {
                using (_pnpClientContext)
                {
                    var group = _pnpClientContext.Web.SiteGroups.GetById(idGroup);
                    User oUser = _pnpClientContext.Web.EnsureUser(user.Email);
                    _pnpClientContext.ExecuteQuery();
                    group.Users.AddUser(oUser);
                    _pnpClientContext.ExecuteQueryAsync();
                }

            }
            catch (Exception ex)
            {

                throw (ex);
            }
        }
    }
     
}
