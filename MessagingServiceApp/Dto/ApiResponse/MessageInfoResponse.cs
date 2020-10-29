using System;

namespace MessagingServiceApp.Dto.ApiResponse
{
    public class MessageInfoResponse
    {
        public string SenderUserUserName { get; set; }
        public string ContactUserUserName { get; set; }
        public string MessageText { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}