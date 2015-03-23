using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Security.Claims;
using System.Threading;
using System.Web;


using Overlord.Security.Claims;
using Overlord.Security.ClaimTypes;

namespace Overlord.Security
{
    public static class OverlordIdentity
    {
        #region Private static methods
        private static void InitalizeIdentity()
        {
            ClaimsIdentity current_user_identity = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(current_user_identity, current_user_identity.Claims, current_user_identity.AuthenticationType,
                current_user_identity.NameClaimType, ClaimTypes.Authentication.Role));
            Thread.CurrentPrincipal = principal;
            ClaimsIdentity new_user_identity = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;
            foreach (Claim c in new_user_identity.Claims.Where(c => c.Type == ClaimTypes.Authentication.Role))
            {
                new_user_identity.RemoveClaim(c);
            }
            new_user_identity.AddClaim(new Claim(Authentication.Role, UserRole.Anonymous));
        }

        #endregion

        #region Private static properties
        public static bool Initialized {get; set;}

        #endregion

        #region Public static methods
        public static void InitAdminUser(string admin_user_id, string admin_user_token)
        {
            if (!Initialized) InitalizeIdentity();
            if (string.IsNullOrEmpty(admin_user_id))
            {
                throw new ArgumentNullException("Admin User Id is null or empty.");                
            }
            ClaimsIdentity user_identity = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;            
            List<Claim> claims = new List<Claim>()
            {                                                
                new Claim(ClaimTypes.Authentication.AdminUserId, admin_user_id),
                new Claim(ClaimTypes.Authentication.AdminUserToken, admin_user_token),
                new Claim(ClaimTypes.Authentication.Role, UserRole.Administrator)                                                        
            };
            user_identity.AddClaims(claims);
        }
        
        public static void InitalizeAnonymousUserIdentity()
        {
            if (!Initialized) InitalizeIdentity();
            ClaimsIdentity user_identity = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;            
            foreach (Claim c in user_identity.Claims.Where(c => c.Type == ClaimTypes.Authentication.Role))
            {
                    user_identity.RemoveClaim(c);
            }
            user_identity.AddClaim(new Claim(Authentication.Role, UserRole.Anonymous));
        }
        
        public static void InitalizeUserIdentity(string user_id, string user_token, string[] user_devices)
        {
            if (!Initialized) InitalizeIdentity();
            if (string.IsNullOrEmpty(user_id))
            {
                throw new ArgumentNullException("User Id is null or empty.");                                
            }
            ClaimsIdentity user_identity = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;
            foreach (Claim c in user_identity.Claims.Where(c => c.Type == ClaimTypes.Authentication.Role))
            {
                user_identity.RemoveClaim(c);
            }
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Authentication.UserId, user_id),
                new Claim(ClaimTypes.Authentication.UserToken, user_token),                                                                                     
            };
            foreach (string d in user_devices)
            {
                claims.Add(new Claim(Authentication.UserDevice, d));
            }
            user_identity.AddClaims(claims);                     
        }
        
        public static void InitializeDeviceIdentity(string device_id, string device_token, 
            string[] device_sensors)
        {
            if (!Initialized) InitalizeIdentity();
            if (string.IsNullOrEmpty(device_id) || string.IsNullOrEmpty(device_token))
            {
                throw new ArgumentNullException("Device Id or Token is null or empty.");                                
            }
            ClaimsIdentity device_identity = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;
            foreach (Claim c in device_identity.Claims.Where(c => c.Type == ClaimTypes.Authentication.Role))
            {
                device_identity.RemoveClaim(c);
            }
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Authentication.DeviceId, device_id),
                new Claim(ClaimTypes.Authentication.DeviceToken, device_token)                                                                    
            };
            foreach (string s in device_sensors)
            {
                claims.Add(new Claim(Authentication.DeviceSensor, s));
            }
            device_identity.AddClaims(claims);
        }

        public static string CurrentDeviceId
        {
            get
            {
                ClaimsIdentity userIdentity = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;
                Claim device_id = userIdentity.Claims.FirstOrDefault(c => 
                    c.Type == ClaimTypes.Authentication.DeviceId);
                if (device_id == null) throw new 
                    InvalidOperationException("Could not retrieve device id claim from identity.");
                else return device_id.Value;
            }
        }
        
        public static bool IsInRole(string role)
        {
            ClaimsPrincipal claimsPrincipal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            return claimsPrincipal.IsInRole(role);
        }

        public static bool HasClaim(string ClaimType)
        {
            ClaimsIdentity identity = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;
            if (identity.HasClaim(c => c.Type == ClaimType))
            {
                return true;
            }
            else return false;
        }

        public static bool HasClaim(string claim_type, string claim)
        {
            ClaimsIdentity identity = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;
            if (identity.HasClaim(claim_type, claim))
            {
                return true;
            }
            else return false;
        }

        public static bool UserHasDevice(string device_id)
        {
            
            ClaimsIdentity identity = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;
            if (identity.HasClaim(ClaimTypes.Authentication.UserDevice, device_id))
            {
                return true;
            }
            else return false;
        }

        public static IEnumerable<Claim> GetClaim(string claimType)
        {
            ClaimsIdentity identity = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;
            IEnumerable<Claim> claim = identity.Claims.Where(c => c.Type == claimType);
            return claim;
            //if (claim != null)
            //{
            //    return claim;
            //}
            //else throw new ArgumentNullException("Identity does not have claim " + claimType);
        }

        public static void AddClaim(string claimType, string claimValue)
        {
            if (string.IsNullOrEmpty(claimType) || string.IsNullOrEmpty(claimValue))
            {
                throw new ArgumentNullException("Claim type or claim value is null.");
            }
            else
            {
                ClaimsIdentity claims_identity = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;
                Claim claim = new Claim(claimType, claimValue);
                claims_identity.AddClaim(claim);
            }
        }

        public static void DeleteClaim(string claim_type, string claim_value)
        {
            if (string.IsNullOrEmpty(claim_type) | string.IsNullOrEmpty(claim_value))
            {
                throw new ArgumentNullException("Claim type or claim value is null.");
            }
            else
            {
                ClaimsIdentity claims_identity = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;
                claims_identity.RemoveClaim(claims_identity.Claims.Where(c => c.Type == claim_type && c.Value == claim_value).FirstOrDefault());                
            }
        }

        #endregion
    }
}