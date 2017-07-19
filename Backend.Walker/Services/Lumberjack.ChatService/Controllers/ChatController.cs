using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.WebSockets;
using Lumberjack.ChatService.Handlers;

namespace Lumberjack.ChatService.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    public class ChatController : ApiController
    {
        public HttpResponseMessage Get()
        {
            var context = HttpContext.Current;
            if (!context.IsWebSocketRequest)
                return Request.CreateResponse(HttpStatusCode.InternalServerError);

            context.AcceptWebSocketRequest(WebSocketRequest);
            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
        }

        private Task WebSocketRequest(AspNetWebSocketContext context)
        {
            var username = "Guest";
            if (User.Identity.IsAuthenticated)
                username = User.Identity.Name;

            var handler = new WebSocketChatHandler(username);
            return handler.ProcessWebSocketRequestAsync(context);
        }
    }
}