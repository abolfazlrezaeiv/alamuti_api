using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Advertisement : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public byte[] photo { get; set; }
        public DateTime? DatePosted { get; set; }
        public string UserId { get; set; }
  

    }
}
