using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Fireflow.Security.Owin.Instagram
{
    public class InstagramAuthenticationOptions : AuthenticationOptions
    {
        public InstagramAuthenticationOptions()
            : base("Instagram")
        {
            this.Caption = "Instagram";
            this.CallbackPath = new PathString("/signin-instagram");
            this.BackchannelTimeout = TimeSpan.FromSeconds(60.0);
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public string Caption
        {
            get => Description.Caption;
            set => Description.Caption = value;
        }

        public PathString CallbackPath { get; set; }
        public TimeSpan BackchannelTimeout { get; set; }
        public string SignInAsAuthenticationType { get; set; }
        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
    }
}