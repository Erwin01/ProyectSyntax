namespace API.Controllers
{
    using SERVICES;
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Cors;

    [RoutePrefix("api/entity")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EntityController : ApiController
    {
        [HttpPost]
        [Route("{entityName}/add/{id}")]
        public void Create([FromUri]string entityName, [FromUri]string id)
        {
            Task.Run(() => 
            {
                var entityServiceClient = new EntityService();
                entityServiceClient.CreateEntity(entityName, Guid.Parse(id));
            });
        }
    }
}