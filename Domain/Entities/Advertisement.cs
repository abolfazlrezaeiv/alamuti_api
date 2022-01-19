using System;

namespace Domain.Entities
{
    public class Advertisement : IEntity
    {
        public int Id { get; set; }
        public string AdsType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public byte[] Photo1 { get; set; }
        public byte[] Photo2 { get; set; }
        public byte[] ListViewPhoto { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.UtcNow ;
        public string UserId { get; set; }
        public bool Published { get; set; } = false;
        public string Village { get; set; }
        public int? Area { get; set; }
    }
}
