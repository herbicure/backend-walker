using System;
using Fireflow.Security;

namespace Lumberjack.ChatService.Models
{
    public class ChatMessage
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public DateTime Submitted { get; set; }
        public ApplicationUser From { get; set; }
    }
}