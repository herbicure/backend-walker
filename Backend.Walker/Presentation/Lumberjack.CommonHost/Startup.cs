using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Newtonsoft.Json.Serialization;
using Owin;

[assembly: OwinStartup(typeof (Lumberjack.CommonHost.Startup))]

namespace Lumberjack.CommonHost
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            AutofacConfig.RegisterResolver(config);

            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(DefaultAuthenticationTypes.ExternalBearer));

            ConfigureAuth(app);
            ConfigureSerializationFormatter(config);

            app.UseWebApi(config);
            WebApiConfig.Register(config);
        }

        private static void ConfigureSerializationFormatter(HttpConfiguration config)
        {
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}