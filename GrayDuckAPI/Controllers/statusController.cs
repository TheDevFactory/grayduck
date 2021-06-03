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
            return new string[] { "pong" };
        }

    }
}
