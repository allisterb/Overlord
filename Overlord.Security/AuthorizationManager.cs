using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel;
using System.Linq;
using System.Threading;
using System.Web;

namespace Overlord.Security
{
    public class AuthorizationManager : ClaimsAuthorizationManager
    {
        public AuthorizationManager() {}
        public override bool CheckAccess(AuthorizationContext context)
        {                        
            return OverlordIdentity.HasClaim((string) context.Resource.First().Value, (string) context.Action.First().Value);            
        }
    }
}