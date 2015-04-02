using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using Overlord.Http.Filters;
using Overlord.Http;
using Overlord.Security;
using Overlord.Security.Claims;
using Overlord.Core;

namespace Overlord.Http.Controllers
{    
    [OverlordAuthenticate, OverlordHttpAction]
    [Authorize(Roles = UserRole.Device)]    
    public class DeviceReadingController : ApiController
    {
        //public static HttpEventSource Log = HttpEventSource.Log;  

        [HttpGet, Route("api/v1/device/test")]
        public async Task<IHttpActionResult> Test()
        {
            
            return Ok();
        }
        [HttpGet, Route("api/v1/device/addreading/time/{time}/values/{values}")]
        public async Task<IHttpActionResult> AddReading(DateTime time, IDictionary<string, object> sensor_values)
        {
            return Ok();
        }
    }
}
