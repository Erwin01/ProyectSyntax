namespace SERVICES
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using ViewModels = Models;
    using System.Linq;
    public class SPBaseService : MainService
    {
        private ViewModels.OAuth2Token token;
        private ViewModels.SPContextInfo spContextInfo;

        public ViewModels.OAuth2Token Token
        {
            get { return token; }
        }
        public ViewModels.SPContextInfo SPContext
        {
            get { return spContextInfo; }
        }


        public SPBaseService():base()
        {
            token = new OAuth2Service().GetAuth2TokenSP();
            this.spContextInfo = this.GetRQDigest();
        }    
        private ViewModels.SPContextInfo GetRQDigest()
        {
            string url = string.Format(Constants.FORMATS.SPROUTES.SPCONTEXT.INFO, this.Config[Constants.APPSETTINGS.SITE_URL]);
            var digest = Utils.CallRESTService<ViewModels.SPDefaultData<ViewModels.SPWebContext>>(
                  url: url,
                  method: HttpRestMethod.POST,
                  authorization: Constants.AUTHORIZATION.BEARER,
                  token: token.AccessToken,
                  headers: new Dictionary<string, string>
                  {
                    {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE}

                  }
                );
            if(digest != null)
            {
                return digest.Data.GetContextWebInformation;
            }
            return null;
        }
        public HttpContent CreateBatchContentItem<T>(ICollection<ViewModels.SPBatchDefinition<T>> batchItemDefs)
        {
            HttpContent dataContent = null;
            var batchItemDef = batchItemDefs.FirstOrDefault();
            switch (batchItemDef.Method)
            {
                case HttpRestMethod.POST:
                case HttpRestMethod.DELETE:
                case HttpRestMethod.UPDATE:
                    {
                        dataContent = new StringContent(CreateBatchItem(batchItemDefs));
                        dataContent.Headers.Remove(Constants.HEADERS.CONTENT_TYPE);
                        dataContent.Headers.TryAddWithoutValidation(Constants.HEADERS.CONTENT_TYPE, string.Format(Constants.MEDIATYPES.MULTIPART_MIXED_CHANGESET, batchItemDef.ChangeSetId));
                    }
                    break;
            }
            return dataContent;
        }
        public List<string> CreateBatchItem<T>(ViewModels.SPBatchDefinition<T> batchItemDef)
        {
            var batchContents = new List<string>();
            switch (batchItemDef.Method)
            {
                case HttpRestMethod.POST:
                case HttpRestMethod.DELETE:
                case HttpRestMethod.UPDATE:
                    {
                        batchContents.Add(string.Format("--changeset_{0}", batchItemDef.ChangeSetId));
                        batchContents.Add("Content-Type: application/http");
                        batchContents.Add("Content-Transfer-Encoding: binary");
                        batchContents.Add(string.Empty);
                        batchContents.Add(string.Format("{0} {1} HTTP/1.1", batchItemDef.Method, batchItemDef.EndPoint));
                        batchContents.Add("Content-Type: application/json;odata=verbose");
                        foreach (var header in batchItemDef.Headers)
                        {
                            batchContents.Add(string.Format("{0}: {1}",header.Key, header.Value));
                        }
                        batchContents.Add(string.Empty);
                        batchContents.Add(batchItemDef.Item != null ? JsonConvert.SerializeObject(batchItemDef.Item): string.Empty);
                        batchContents.Add(string.Empty);
                    }   break;
            }
            if (batchItemDef.IsLastItem)
            {
                batchContents.Add(string.Format("--changeset_{0}--", batchItemDef.ChangeSetId));
            }
            return batchContents;
        }
        public string CreateBatchItem<T>(ICollection<ViewModels.SPBatchDefinition<T>> batchItemDefs)
        {
            var batchContents = new List<string>();
            foreach (var batchItemDef in batchItemDefs)
            {
                switch (batchItemDef.Method)
                {
                    case HttpRestMethod.POST:
                    case HttpRestMethod.DELETE:
                    case HttpRestMethod.UPDATE:
                        {
                            batchContents.AddRange(CreateBatchItem<T>(batchItemDef));
                        }
                        break;
                }
            }
            return string.Join("\r\n", batchContents);
        }

    }
    
  
}
