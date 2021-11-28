using application.DTOs.Requests;
using application.Interfaces.Data;
using application.Interfaces.repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class AdvertisementRepository : IRepository<Advertisement>
    {
        private readonly AlamutDbContext _context;

        public AdvertisementRepository(AlamutDbContext context)
        {
            _context = context;
        }
        public async Task<Advertisement> Add(Advertisement entity)
        {
           
            _context.Advertisements.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

      

        public async Task<Advertisement> Delete(int id)
        {
           
            var entity = await _context.Advertisements.FindAsync(id);
            if (entity == null)
            {
                return entity;
            }

            _context.Advertisements.Remove(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<Advertisement> Get(int id)
        {
            return await _context.Advertisements.FindAsync(id);
        }

        public async Task<IEnumerable<Advertisement>> GetAll()
        {
            return await _context.Advertisements.ToListAsync();
        }

        public async Task<Advertisement> Update(Advertisement entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
