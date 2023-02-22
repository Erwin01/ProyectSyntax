using SERVICES;
using SERVICES.Models;
using System;
using System.Web.Http;
using System.Web.Http.Cors;

namespace API.Controllers
{
    [RoutePrefix("api/schema")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SchemaController : ApiController
    {
        [HttpPost]
        [Route("create")]
        public GenericResponse CreateSchema()
        {
            try
            {
                var service = new SchemaService();
                service.Create();
                return new GenericResponse
                {
                    message = "success",
                    status = "success",
                    code = 200,
                    result = true
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
        [Route("delete")]
        public GenericResponse DeleteSchema()
        {
            try
            {
                var service = new SchemaService();
                service.Delete();
                return new GenericResponse
                {
                    message = "success",
                    status = "success",
                    code = 200,
                    result = true
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