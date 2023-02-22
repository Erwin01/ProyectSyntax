using Newtonsoft.Json;
using SERVICES;
using SERVICES.Models;
using System;
using System.Web.Http;
using System.Web.Http.Cors;

namespace API.Controllers
{

    [RoutePrefix("api/getUser")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UsersController : ApiController
    {


        // GET: Get All Dynamics 365 Users
        [HttpGet]
        [Route("users/{entityUser}")]
        public GenericResponse GetAllDynamicsUsers()
        {


            try
            {
                var service = new SecurityService();

                var result = JsonConvert.SerializeObject(service.GetAllDynamics365Users());

                return new GenericResponse
                {
                    message = "success",
                    status = "success",
                    code = 200,
                    result = result
                };
            }
            catch (Exception ex)
            {

                return new GenericResponse
                {
                    message = ex.Message,
                    status = "success",
                    code = 200,
                    result = false
                };
            }
        }


        //[HttpGet]
        //[Route("bussinessUnit/{entityBussinessUnit}")]
        //public SecurityService GetAllDynamicsBussinessUnit()
        //{

        //    var service = new SecurityService();
        //    service.GetAllDynamics365BusinessUnits();

        //    return service;

        //}


        [HttpGet]
        [Route("bussinessUnit/{entityBussinessUnit}")]
        public GenericResponse GetAllDynamicsBussinessUnit()
        {

            try
            {
                var service = new SecurityService();

                var result = JsonConvert.SerializeObject(service.GetAllDynamics365BusinessUnits());

                return new GenericResponse
                {
                    message = "success",
                    status = "success",
                    code = 200,
                    result = result
                };
            }
            catch (Exception ex)
            {

                return new GenericResponse
                {
                    message = ex.Message,
                    status = "success",
                    code = 200,
                    result = false
                };
            }

        }



        [HttpGet]
        [Route("dynamicsUser365ByBusinessUnit/{entityUserBusinessUnit}")]
        public GenericResponse GetAllDynamicsUser365ByBusinessUnit() 
        {
            try
            {
                var service = new SecurityService();

                var result = JsonConvert.SerializeObject(service.GetAllDynamicsUser365ByBusinessUnit());

                return new GenericResponse 
                {
                    message = "success",
                    status = "success",
                    code = 200,
                    result = result

                };
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    message = ex.Message,
                    status = "success",
                    code = 200,
                    result = false

                };
            }
        }

    }
}