using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ChatMessage : IEntity
    {
        public int Id { get; set; }
        public string Sender { get; set; }
     
        public string Message { get; set; }
        public string Reciever { get; set; }
        public DateTime DateSended { get; set; } = DateTime.UtcNow;
        public string GroupName { get; set; }
        public int ChatGroupId { get; set; }
        public ChatGroup ChatGroup { get; set; }

    }
}
