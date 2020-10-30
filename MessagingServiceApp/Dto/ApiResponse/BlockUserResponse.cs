using System;

namespace MessagingServiceApp.Dto.ApiResponse
{
    public class BlockUserResponse
    {
        public string BlockingUserUserName { get; set; }

        public string BlockedUserUserName { get; set; }

        public bool IsBlocked { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}