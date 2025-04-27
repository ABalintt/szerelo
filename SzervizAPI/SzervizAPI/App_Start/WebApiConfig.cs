using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SzervizAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute(
                origins: "http://localhost:3000",
                headers: "*",
                methods: "*"
            );
            config.EnableCors(cors);

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
            name: "swagger",
            routeTemplate: "",
            defaults: null,
            constraints: null,
            handler: new RedirectHandler((url => url.RequestUri.ToString()), "swagger")
            );
        }
    }
}
