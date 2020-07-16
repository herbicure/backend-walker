using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Dispatcher;
using Lumberjack.CommonHost.Handlers;

namespace Lumberjack.CommonHost
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "SteamRoute",
                routeTemplate: "api/steam",
                defaults: null,
                constraints: null,
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    new DelegatingHandler[] { new SteamHandler() }
                    )
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            //config.MessageHandlers.Add(new SteamHandler());
        }
    }
}