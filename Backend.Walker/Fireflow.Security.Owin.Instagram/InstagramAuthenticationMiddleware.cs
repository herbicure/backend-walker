using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace Fireflow.Security.Owin.Instagram
{
    public class InstagramAuthenticationMiddleware : AuthenticationMiddleware<InstagramAuthenticationOptions>
    {
        private readonly HttpClient _httpClient;

        public InstagramAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, InstagramAuthenticationOptions options)
            : base(next, options)
        {
            if (Options.StateDataFormat == null)
                Options.StateDataFormat = new PropertiesDataFormat(app.CreateDataProtector(typeof(InstagramAuthenticationMiddleware).FullName, Options.AuthenticationType));

            if (string.IsNullOrEmpty(Options.SignInAsAuthenticationType))
                Options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType();


            this._httpClient = new HttpClient
            {
                Timeout = this.Options.BackchannelTimeout,
                MaxResponseContentBufferSize = 10485760L
            };
        }

        protected override AuthenticationHandler<InstagramAuthenticationOptions> CreateHandler()
        {
            return new InstagramAuthenticationHandler(_httpClient);
        }
    }
}