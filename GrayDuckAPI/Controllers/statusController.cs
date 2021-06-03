using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrayDuckAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class statusController : ControllerBase
    {
        // GET: api/<statusController>
        [HttpGet]
        public IEnumerable<string> Get()
        {

            //int intValue = 100;
            //int intValue2 = 0;

            //int intOutputValue = 0;

            ////Lets make it break!
            //intOutputValue = intValue / intValue2;


            return new string[] { "pong" };
        }

    }
}
