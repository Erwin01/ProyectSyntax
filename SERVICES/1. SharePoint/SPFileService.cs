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
    public class SPFileService: SPBaseService
    {
        public SPFileService(): base()
        {

        }
        
        public SPDefaultData<SPDefaultResults<dynamic>> AddFile(HttpContent data, string serverRelariveUrl, string fileName, bool overwrite = false)
        {
            try
            {
                return Utils.CallRESTService<SPDefaultData<SPDefaultResults<dynamic>>>(
                    url: string.Format(Constants.FORMATS.SPROUTES.SPFILE.ADD, 
                        this.Config[Constants.APPSETTINGS.SITE_URL], 
                        serverRelariveUrl,
                        overwrite.ToString().ToLowerInvariant(),
                        fileName
                        ),
                    authorization: Constants.AUTHORIZATION.BEARER,
                    token: this.Token.AccessToken,
                    method:HttpRestMethod.POST,
                    headers: new Dictionary<string, string>
                    {
                        {Constants.HEADERS.ACCEPT, Constants.MEDIATYPES.JSON_ODATA_VERBOSE},
                        {Constants.HEADERS.X_REQUEST_DIGEST,this.SPContext.FormDigestValue },
                    },
                    data: data
                );
            }
            catch (Exception)
            {
                throw;

            }
        }
    }
}
