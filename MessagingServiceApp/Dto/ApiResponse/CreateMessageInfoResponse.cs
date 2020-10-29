using System;

namespace MessagingServiceApp.Dto.ApiResponse
{
    public class CreateMessageInfoResponse
    {
        public string MessageText { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}