using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Advertisement 
    {
        [Key]
        public int Id { get; set; }
        public string AdsType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public byte[] Photo1 { get; set; }
        public byte[] Photo2 { get; set; }
        public byte[] ListViewPhoto { get; set; }
#nullable enable
        public DateTime? DatePosted { get; set; } = DateTime.UtcNow;
        public string? ReportMessage { get; set; }
        public bool? Published { get; set; } = false;
        public string? UserId { get; set; }
#nullable disable

        public string Village { get; set; }
        public int? Area { get; set; }

    }
}
