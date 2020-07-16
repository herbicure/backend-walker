using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace Fireflow.Security.Owin.Instagram
{
    public class InstagramAuthenticationHandler : AuthenticationHandler<InstagramAuthenticationOptions>
    {
        private readonly HttpClient _httpClient;

        public InstagramAuthenticationHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            var properties = (AuthenticationProperties)null;

            try
            {
                var state = (string)null;
                var query = Request.Query;
                var values = query.GetValues("code");

                properties = Options.StateDataFormat.Unprotect(state);
                if (properties != null)
                {
                    throw new NotImplementedException();
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new AuthenticationTicket(null, properties);
            }
        }
    }
}
