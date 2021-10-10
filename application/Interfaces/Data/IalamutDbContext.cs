using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace application.Interfaces.Data
{
    class IAlamutDbContext
    {
        public DbSet<Advertisement> Advertisements { get; set; }
    }
}
