using SERVICES;
using System.Web.Http;
using System.Web.Http.Cors;

namespace API.Controllers
{
    [RoutePrefix("api/security")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SecurityController : ApiController
    {
        [HttpPost]
        [Route("entities/{entityName}")]
        public string Sync([FromUri]string entityName)
        {
            var securityServiceClient = new SecurityService();
            return securityServiceClient.SyncUsers(entityName);
        }

        [HttpDelete]
        [Route("entities/{entityName}")]
        public void DeletePermissions([FromUri]string entityName)
        {
            var securityServiceClient = new SecurityService();
            securityServiceClient.DeletePermissions(entityName);
        }

        [HttpPost]
        [Route("templates")]
        public void SyncTemplates()
        {
            var securityServiceClient = new SecurityService();
            securityServiceClient.SyncUsersTemplates();
        }
    }
}