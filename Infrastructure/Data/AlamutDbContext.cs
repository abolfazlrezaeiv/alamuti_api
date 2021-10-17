using application.Interfaces.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data
{
    class AlamutDbContext : DbContext, IAlamutDbContext
    {
        public DbSet<Advertisement> Advertisements { get; set; }
    }
}
