using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Fireflow.Security;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth;

namespace Lumberjack.CommonHost.Providers
{
    public class JwtAuthorizationProvider : OAuthAuthorizationServerProvider
    {
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] {"*"});
            var manager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            var user = await manager.FindAsync(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            //if (!user.EmailConfirmed)
            //{
            //    context.SetError("invalid_grant", "User did not confirm email.");
            //    return;
            //}

            var identity = new ClaimsIdentity("JWT");
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim("sub", user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, "User"));

            context.Validated(identity);
            context.Request.Context.Authentication.SignIn(identity);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            try
            {
                if (context.ClientId == null)
                    context.Validated();
            }
            catch (Exception)
            {
                context.SetError("Server error");
                context.Rejected();
            }

            return base.ValidateClientAuthentication(context);
        }
    }
}