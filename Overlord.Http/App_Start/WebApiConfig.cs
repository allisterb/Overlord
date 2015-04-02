using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Formatting;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;

namespace Overlord.Http
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only our authentication filter.
            config.SuppressDefaultHostAuthentication();
                     
            // Web API routes
            config.MapHttpAttributeRoutes();

            // Formatters
            config.Formatters.XmlFormatter.MediaTypeMappings.Add(
               new QueryStringMapping("format", "xml", "application/xml"));
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
           
        }
    }
}
