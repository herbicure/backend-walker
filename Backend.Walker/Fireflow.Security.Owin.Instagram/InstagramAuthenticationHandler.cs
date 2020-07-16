using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Net.Http;
using System.Threading.Tasks;

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
