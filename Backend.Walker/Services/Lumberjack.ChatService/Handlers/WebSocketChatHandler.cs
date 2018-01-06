using System;
using System.Threading;
using Lumberjack.ChatService.Models;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;

namespace Lumberjack.ChatService.Handlers
{
    public class WebSocketChatHandler : WebSocketHandler
    {
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
        private static readonly WebSocketCollection ChatClients = new WebSocketCollection();
        private readonly string _username;

        public WebSocketChatHandler(string username)
        {
            this._username = username;
        }

        public override void OnOpen()
        {
            Locker.EnterWriteLock();
            try
            {
                ChatClients.Add(this);
                var message = new { type = "CONNECTION", text = $"{_username} joined." };
                ChatClients.Broadcast(JsonConvert.SerializeObject(message));
            }
            finally
            {
                Locker.ExitWriteLock();
            }
        }

        public override void OnMessage(string message)
        {
            var msg = JsonConvert.DeserializeObject<ChatMessage>(message);
            msg.Submitted = DateTime.Now;
            msg.From.UserName = _username;

            ChatClients.Broadcast(JsonConvert.SerializeObject(msg));
        }

        public override void OnClose()
        {
            Locker.EnterWriteLock();
            try
            {
                ChatClients.Remove(this);
                var message = new { type = "CONNECTION", text = $"{_username} left." };
                ChatClients.Broadcast(JsonConvert.SerializeObject(message));
            }
            finally
            {
                Locker.ExitWriteLock();
            }
        }
    }
}