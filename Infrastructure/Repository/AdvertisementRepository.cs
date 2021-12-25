using application.DTOs.Requests;
using application.Interfaces.Data;
using application.Interfaces.repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class AdvertisementRepository : IAdvertisementRepository
    {
        private readonly AlamutDbContext _context;

        public AdvertisementRepository(AlamutDbContext context)
        {
            _context = context;
        }
        public async Task<Advertisement> Add(Advertisement entity)
        {
            await _context.Advertisements.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }


        public async Task<Advertisement> Delete(Advertisement entity)
        {

            var entityInDatabase = await _context.Advertisements.FindAsync(entity.Id);
            if (entityInDatabase == null)
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


        public async Task<IEnumerable<Advertisement>> GetCurrentUserAds(IdentityUser user)
        {
            return await _context.Advertisements.Where(x=>x.UserId == user.Id).ToListAsync();
        }



        public async Task<IEnumerable<Advertisement>> Find(string input)
        {

            return await _context.Advertisements.Where(x => x.Title.Contains(input)).ToListAsync();
        }


        public async Task<IEnumerable<Advertisement>> GetAll(string adsType)
        {

            return await _context.Advertisements.Where(x=>x.AdsType == adsType).ToListAsync();
        }


        public async Task<IEnumerable<Advertisement>> GetAll()
        {

            return await _context.Advertisements.ToListAsync();
        }


        public async Task<Advertisement> Update(Advertisement entity)
        {
          
                var result = _context.Advertisements.SingleOrDefault(b => b.Id == entity.Id);
                if (result != null)
                {
                    result.Title = entity.Title;
                    result.Area = entity.Area;
                    result.Price = entity.Price;
                    result.Description = entity.Description;  
                    result.photo1 = entity.photo1 ;
                    result.photo2 = entity.photo2 ;
                    await _context.SaveChangesAsync();
                }
            return entity;
        }
    }
}
