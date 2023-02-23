namespace SERVICES
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using ViewModels = Models;
    using System.Linq;
    public class SPListService : SPBaseService
    {
        public ViewModels.SPDefaultData<ViewModels.SPDefaultResults<T>> GetItems<T>(string listTitle, string query = "")
        {
            try
            {
                return Utils.CallRESTService<ViewModels.SPDefaultData<ViewModels.SPDefaultResults<T>>>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPLIST.GETITEMS, this.Config[Constants.APPSETTINGS.SITE_URL], listTitle, query),
                    method: HttpRestMethod.GET,
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    headers: new Dictionary<string, string>
                    {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE},
                    }
                );
            }
            catch (Exception)
            {
                throw;

            }
        }
        public dynamic Update<T>(string listTitle,string idItem, T itemInfo)
        {
            try
            {
                var json = JsonConvert.SerializeObject(itemInfo);
                var content = new StringContent(json.ToString());
                content.Headers.ContentType = MediaTypeHeaderValue.Parse(Constants.MEDIATYPES.JSON_ODATA_VERBOSE);
                var query = String.Format("({0})", idItem);
                return Utils.CallRESTService<ViewModels.SPDefaultData<ViewModels.SPDefaultResults<T>>>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPLIST.GETITEMS, this.Config[Constants.APPSETTINGS.SITE_URL], listTitle, query),
                    method: HttpRestMethod.POST,
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    headers: new Dictionary<string, string>
                    {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE},
                        {Constants.HEADERS.X_REQUEST_DIGEST,this.SPContext.FormDigestValue },
                        {Constants.HEADERS.X_HTTP_METHOD, HttpRestMethod.MERGE.ToString() },
                        {Constants.HEADERS.IF_MATCH, "*" }
                    },
                    data:content
                );
            }
            catch (Exception)
            {
                throw;

            }
        }
        public dynamic Create<T>(string listTitle, T itemInfo)
        {
            try
            {
                var json = JsonConvert.SerializeObject(itemInfo);
                var content = new StringContent(json.ToString());
                content.Headers.ContentType = MediaTypeHeaderValue.Parse(Constants.MEDIATYPES.JSON_ODATA_VERBOSE);
                return Utils.CallRESTService<ViewModels.SPDefaultData<ViewModels.SPDefaultResults<T>>>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPLIST.GETITEMS, this.Config[Constants.APPSETTINGS.SITE_URL], listTitle, string.Empty),
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

        
        public dynamic Delete(string listTitle, string idItem)
        {
            try
            {
                var query = String.Format("({0})", idItem);
                return Utils.CallRESTService<dynamic>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPLIST.GETITEMS, this.Config[Constants.APPSETTINGS.SITE_URL], listTitle, query),
                    method: HttpRestMethod.POST,
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    headers: new Dictionary<string, string>
                    {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE},
                        {Constants.HEADERS.X_REQUEST_DIGEST,this.SPContext.FormDigestValue },
                        {Constants.HEADERS.X_HTTP_METHOD, HttpRestMethod.DELETE.ToString() },
                        {Constants.HEADERS.IF_MATCH, "*" }
                    }
                );
            }
            catch (Exception)
            {
                throw;

            }
        }
        public ViewModels.SPDefaultData<ViewModels.SPList> CreateLibrary(string listTitle, string subSiteUrl, string query = "")
        {
            try
            {
                var listInfo = new
                {
                    __metadata = new { type = "SP.List"},
                    Title = listTitle,
                    Description = listTitle,
                    BaseTemplate = 101
                };
                var json = JsonConvert.SerializeObject(listInfo);
                var content = new StringContent(json.ToString());
                content.Headers.ContentType = MediaTypeHeaderValue.Parse(Constants.MEDIATYPES.JSON_ODATA_VERBOSE);
                return Utils.CallRESTService<ViewModels.SPDefaultData<ViewModels.SPList>>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPLIST.GENERAL, this.Config[Constants.APPSETTINGS.SITE_URL] + subSiteUrl, query),
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
        public dynamic BreakRoleInheritance(string listTitle, string subSiteUrl, bool clearPermissions = true)
        {
            try
            {
                return Utils.CallRESTService<dynamic>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPLIST.BREAK_ROLE_INHERITANCE, this.Config[Constants.APPSETTINGS.SITE_URL] + subSiteUrl, listTitle, (!clearPermissions).ToString().ToLowerInvariant()),
                    method: HttpRestMethod.POST,
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    headers: new Dictionary<string, string>
                    {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE},
                        {Constants.HEADERS.X_REQUEST_DIGEST,this.SPContext.FormDigestValue }
                    },
                    data: null
                );
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public dynamic AddRoleAssigment(int principalId, string listTitle, string roleDefId, string subSiteUrl)
        {
            var url = String.Format(Constants.FORMATS.GENERAL.URL_SEPARATOR, this.Config[Constants.APPSETTINGS.SITE_URL], subSiteUrl);
            return Utils.CallRESTService<dynamic>(
                url: string.Format(Constants.FORMATS.SPROUTES.SPLIST.ADD_ROLE_ASSIGMENT, url, listTitle, principalId, roleDefId),
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
        }
        public Stream DeleteItemsAsBatch(string listTitle, ICollection<dynamic> items)
        {
            try
            {
                var batchId = Guid.NewGuid();
                var changeSetId = Guid.NewGuid();
                var content = new MultipartContent("mixed", string.Format("batch_{0}", batchId));
                var batchItemDefs = items.Select((item, index) => new ViewModels.SPBatchDefinition<dynamic>
                {
                    ChangeSetId = changeSetId,
                    EndPoint = string.Format(Constants.FORMATS.SPROUTES.SPLIST.DELETE_ITEM_BY_ID, this.Config[Constants.APPSETTINGS.SITE_URL], listTitle, item.Id),
                    Method = HttpRestMethod.DELETE,
                    IsLastItem = index == items.Count() - 1,
                    Headers = new Dictionary<string, string> { { Constants.HEADERS.IF_MATCH, "*"} }
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
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
