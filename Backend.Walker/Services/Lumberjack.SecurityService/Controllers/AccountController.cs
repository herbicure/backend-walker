using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Fireflow.Security;
using Lumberjack.SecurityService.Models;
using Lumberjack.SecurityService.Results;

namespace Lumberjack.SecurityService.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        public static string PublicClientId => "self";
        protected virtual ApplicationUserManager UserManager => Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
        protected virtual IAuthenticationManager Authentication => Request.GetOwinContext().Authentication;


        [AllowAnonymous]
        [Route("signup")]
        public async Task<IHttpActionResult> Signup(SignupUserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                Authentication.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);
            }

            return Ok();
        }

        [AllowAnonymous]
        [Route("external-logins")]
        public IEnumerable<ExternalLoginModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            var providers = Authentication.GetExternalAuthenticationTypes();
            var logins = new List<ExternalLoginModel>();

            string state = null;

            foreach (var description in providers)
            {
                var login = new ExternalLoginModel
                {
                    Name = description.Caption,
                    Url = Url.Link("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state
                    }),
                    State = state
                };

                logins.Add(login);
            }

            return logins;
        }

        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("external-login", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
                return BadRequest(Uri.EscapeDataString(error));


            if (!User.Identity.IsAuthenticated)
                return new ChallengeResult(provider, this);

            var externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
                return InternalServerError();

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            var userInfo = new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey);
            var user = await UserManager.FindAsync(userInfo);

            var hasRegistered = user != null;
            if (hasRegistered)
            {
                // TODO: Generate user identity claims for registrated user login and signin user to system with claims.
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);


            }
            else
            {
                var claims = externalLogin.GetClaims();
                var identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            var redirectUri = BuildRedirectUri(externalLogin, hasRegistered);
            return Redirect(redirectUri);
        }
        

        #region Private Helpers

        private string BuildRedirectUri(ExternalLoginData externalLogin, bool hasLocalAccount)
        {
            var redirectUri = GetQueryString(Request, "redirect_uri");
            redirectUri += $"signup/{GetQueryString(Request, "provider").ToLower()}";
            redirectUri =
                $"{redirectUri}#external_access_token={externalLogin.ExternalAccessToken}&provider={externalLogin.LoginProvider}&haslocalaccount={hasLocalAccount}&external_user_name={externalLogin.UserName}";
            return redirectUri;
        }

        private string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string ExternalAccessToken { get; set; }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                    return null;

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || string.IsNullOrEmpty(providerKeyClaim.Issuer) ||
                    string.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name),
                    ExternalAccessToken = identity.FindFirstValue("ExternalAccessToken"),
                };
            }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }
        }

        #endregion
    }
}