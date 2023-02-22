using SERVICES;
using SERVICES.Models;
using System;
using System.Web.Http;
using System.Web.Http.Cors;

namespace API.Controllers
{
    [RoutePrefix("api/sync")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SyncController : ApiController
    {
        [HttpPost]
        [Route("entities/{entityName}")]
        public GenericResponse SyncEntity([FromUri]string entityName, [FromBody]SyncEntityRequest request)
        {
            try
            {
                var service = new SyncService();
                return new GenericResponse
                {
                    message = "success",
                    status = "success",
                    code = 200,
                    result = service.SyncEntity(entityName, request.InitDate, request.EndDate)
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    message = ex.Message,
                    status = "fail",
                    code = 500,
                    result = ex
                };
            }
        }

        [HttpPost]
        [Route("templates")]
        public GenericResponse SyncTemplates()
        {
            try
            {
                var service = new SyncService();
                return new GenericResponse
                {
                    message = "success",
                    status = "success",
                    code = 200,
                    result = service.SyncTemplates()
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    message = ex.Message,
                    status = "fail",
                    code = 500,
                    result = ex
                };
            }
        }
    }
}