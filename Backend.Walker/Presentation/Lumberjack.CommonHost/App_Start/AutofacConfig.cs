using System.Web.Http;

namespace Lumberjack.CommonHost
{
    public class AutofacConfig
    {
        public static void RegisterResolver(HttpConfiguration config)
        {
            var resolver = config.DependencyResolver;
        }
    }
}