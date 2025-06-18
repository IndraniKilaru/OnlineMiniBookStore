using System;

namespace BookStore.Shared.Events
{
    public class UserRegisteredEvent
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
    }
}
