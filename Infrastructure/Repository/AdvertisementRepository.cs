using Alamuti.Domain.Entities;
using Alamuti.Infrastructure.Repository;
using application.DTOs;
using application.DTOs.Advertisement;
using application.Interfaces.repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class AdvertisementRepository : GenericRepository<Advertisement>, IAdvertisementRepository
    {
        private readonly AlamutDbContext _context;

        public AdvertisementRepository(AlamutDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> Delete(int adsId, string userId)
        {
            var ads = await _context.Advertisements.FirstOrDefaultAsync(x => x.Id == adsId);
            if (ads == null || ads.UserId != userId) return false;
            _context.Advertisements.Remove(ads);
            return true;
        }

        public async Task<PaginatedList<Advertisement>> Search(string input, AdvertisementParameters advertisementParameters)
        {
            return await PaginatedList<Advertisement>
                .CreateAsync(_context.Advertisements.AsNoTracking().Where(x => (x.Title.Contains(input) || x.Village.Contains(input)) && x.Published == true),
                advertisementParameters.PageNumber,
                advertisementParameters.PageSize);
        }
        public async Task<PaginatedList<Advertisement>> FilterByType(string adsType, AdvertisementParameters advertisementParameters)
        {
            return await PaginatedList<Advertisement>
                  .CreateAsync(_context.Advertisements.AsNoTracking().Where(x => x.AdsType == adsType && x.Published == true)
                  .OrderByDescending(x => x.DatePosted),
                  advertisementParameters.PageNumber,
                  advertisementParameters.PageSize);
        }

        public async Task<PaginatedList<Advertisement>> All(AdvertisementParameters advertisementParameters)
        {
           return await PaginatedList<Advertisement>
                 .CreateAsync(_context.Advertisements.AsNoTracking().Where(x => x.Published == true)
                 .OrderByDescending(x => x.DatePosted),
                 advertisementParameters.PageNumber,
                 advertisementParameters.PageSize);
        }

        public async Task<PaginatedList<Advertisement>> GetUserAds(AlamutiUser user, AdvertisementParameters advertisementParameters)
        {
            return await PaginatedList<Advertisement>
              .CreateAsync(_context.Advertisements.AsNoTracking().Where(x => x.UserId == user.Id),
              advertisementParameters.PageNumber,
              advertisementParameters.PageSize);
        }
        

        public async Task<bool> Update(string userId,Advertisement updatedData)
        {
            if (updatedData.UserId != userId) return false;
            _context.Advertisements.Attach(updatedData);
            updatedData.Published = false;
            var entry = _context.Entry(updatedData);
            entry.Property(e => e.Title).IsModified = true;
            entry.Property(e => e.Area).IsModified = true;
            entry.Property(e => e.Price).IsModified = true;
            entry.Property(e => e.Description).IsModified = true;
            entry.Property(e => e.Photo1).IsModified = true;
            entry.Property(e => e.Photo2).IsModified = true;
            entry.Property(e => e.ListViewPhoto).IsModified = true;
            entry.Property(e => e.Village).IsModified = true;
            entry.Property(e => e.Published).IsModified = true;
            return true;
        }

        public async Task<bool> InsertReport(int id , string report)
        {
            var result = await _context.Advertisements.FirstOrDefaultAsync(x => x.Id == id);
            if (result == null) return false;
            result.ReportMessage = report;
            result.Published = true;
            return true;    
        }
    }
}
