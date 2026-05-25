using System;

namespace AquaPark.Models
{
    public class ActionLogEntry
    {
        public int ActionLogId { get; set; }

        public int? UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public DateTime ActionDate { get; set; }

        public string ActionType { get; set; } = string.Empty;

        public string EntityName { get; set; } = string.Empty;

        public int? EntityId { get; set; }

        public string Details { get; set; } = string.Empty;
    }
}
