using System;
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

        //public async Task<List<Mom>>
    }
}
