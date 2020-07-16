using System;
using Owin;

namespace Fireflow.Security.Owin.Instagram
{
    public static class InstagramExtensions
    {
        public static IAppBuilder UseInstagramAuthentication(this IAppBuilder app, InstagramAuthenticationOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (options == null)
                throw new ArgumentException(nameof(options));

            app.Use(typeof(InstagramAuthenticationMiddleware), app, options);

            return app;
        }
    }
}