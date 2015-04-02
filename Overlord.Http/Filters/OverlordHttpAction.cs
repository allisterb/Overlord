using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Overlord.Http.Filters
{
    public class OverlordHttpAction : ActionFilterAttribute
    {   
        
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            ClaimsPrincipal principal = (ClaimsPrincipal) actionExecutedContext.ActionContext.ControllerContext.RequestContext.Principal;
            if (principal != null) 
            {
                principal = null;
            }
        }
    }
}