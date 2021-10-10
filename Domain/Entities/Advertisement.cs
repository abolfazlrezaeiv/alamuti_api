﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    class Advertisement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public byte[] Photo { get; set; }
        public DateTime DatePosted { get; set; }

    }
}
