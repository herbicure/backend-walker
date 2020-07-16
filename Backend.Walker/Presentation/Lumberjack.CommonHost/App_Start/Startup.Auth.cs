using System;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using Aminjam.Owin.Security.Instagram;
using Fireflow.Security;
using Lumberjack.CommonHost.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Lumberjack.CommonHost
{
    public partial class Startup
    {
        private string Issuer => ConfigurationManager.AppSettings["issuer"];
        private byte[] Secret => TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["secret"]);

        public OAuthAuthorizationServerOptions OAuthOptions { get; set; }

        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(SecurityDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            //ConfigureOAuth(app);
            ConfigureOAuthTokenGeneration(app);
            ConfigureJwtBearerTokenConsumption(app);

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "59342683538-o8aig2822d3qsmp78a343lomk91jdrm9.apps.googleusercontent.com",
                ClientSecret = "d7p8WacVn3x30zcmpqQaq-_N",
                Provider = new GoogleOAuth2AuthenticationProvider
                {
                    OnAuthenticated = async ctx =>
                    {
                        ctx.Identity.AddClaim(new Claim("ExternalAccessToken", ctx.AccessToken));
                        await Task.FromResult<object>(null);
                    },
                }
            });

            app.UseInstagramAuthentication(new InstagramAuthenticationOptions
            {
                ClientId = "a8e71f09a1c04b928fe10c7124544b39",
                ClientSecret = "226347fa74b140ffb5aeea175c0b851b",
                Provider = new InstagramAuthenticationProvider
                {
                    OnAuthenticated = async ctx =>
                    {
                        ctx.Identity.AddClaim(new Claim("ExternalAccessToken", ctx.AccessToken));
                        await Task.FromResult<object>(null);
                    }
                }
            });
        }

        [Obsolete]
        public void ConfigureOAuth(IAppBuilder app)
        {
            // Configure the application for OAuth based flow
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                Provider = new ApplicationOAuthProvider("self"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                //AuthorizeEndpointPath = new PathString("/api/account/external-login")
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthOptions);
            // Token Consumption
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            // Enable the application to use bearer tokens to authenticate users
            //app.UseOAuthBearerTokens(OAuthOptions);
        }

        public void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            // Configure the application for OAuth based flow
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                AuthenticationType = DefaultAuthenticationTypes.ExternalBearer,
                TokenEndpointPath = new PathString("/oauth2/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(60),
                Provider = new JwtAuthorizationProvider(),
                AccessTokenFormat = new JwtDataFormat(Issuer)
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthOptions);
        }

        public void ConfigureJwtBearerTokenConsumption(IAppBuilder app)
        {
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ExternalBearer,
                AuthenticationMode = AuthenticationMode.Active,
                AllowedAudiences = new[] {"Any"},
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                {
                    new SymmetricKeyIssuerSecurityTokenProvider(Issuer, Secret)
                }
            });
        }
    }
}