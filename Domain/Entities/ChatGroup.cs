using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ChatGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public bool IsChecked { get; set; }
        public List<ChatMessage> Messages { get; set; }

    }
}
