using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace TBA.Api.Controllers
{
    [Route("api")]
    [ApiController]
    [FamilyAuthentication]
    public class ApiController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ApiController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [Route("")]
        [Route("index")]
        public string Index()
        {
            return $"All good -- Current local time is {DateTime.Now.ToLongTimeString()}";
        }

        [HttpGet]
        [Route("gt")]
        public ObjectResult GatewayTimeout()
        {
            return StatusCode((int)HttpStatusCode.GatewayTimeout, null);
        }

        [HttpGet]
        [Route("gt2")]
        public IActionResult GatewayTimeout2()
        {
            //return Ok();
            throw new Exception("504");
        }

        //public async Task<List<Mom>>
    }
}
