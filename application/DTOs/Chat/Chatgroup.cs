using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.DTOs.Chat
{
    public class ChatGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public bool IsChecked { get; set; }
        public bool IsDeleted { get; set; }
#nullable enable
        public string? ReportMessage { get; set; }
        public string? BlockedUserId { get; set; }
        public string? Image { get; set; }
#nullable disable
        public ChatMessageDto LastMessage { get; set; }
    }
}
