using System.Collections.Generic;

namespace Domain.Entities
{
    public class ChatGroup : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public bool IsChecked { get; set; } 
#nullable enable
        public string? Image { get; set; }
#nullable disable
        public ICollection<ChatMessage> Messages { get; set; }

        public ChatGroup()
        {
            Messages = new HashSet<ChatMessage>();
        }
    }
}
