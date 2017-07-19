using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Security;

namespace Lumberjack.SecurityService.Results
{
    public class ChallengeResult : IHttpActionResult
    {
        public string LoginProvider { get; }
        public HttpRequestMessage MessageRequest { get; }

        public ChallengeResult(string loginProvider, ApiController controller)
        {
            LoginProvider = loginProvider;
            Options = new AuthenticationProperties();
            MessageRequest = controller.Request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var manager = MessageRequest.GetOwinContext().Authentication;
            manager.Challenge(Options, LoginProvider);

            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                RequestMessage = MessageRequest
            };

            return Task.FromResult(response);
        }

        public AuthenticationProperties Options { get; }
    }
}