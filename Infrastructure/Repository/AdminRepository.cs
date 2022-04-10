using Alamuti.Application.Interfaces.repository;
using application.DTOs;
using application.DTOs.Advertisement;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Alamuti.Infrastructure.Repository
{
    public class AdminRepository : GenericRepository<Advertisement>, IAdminRepository
    {
        private readonly AlamutDbContext _context;
        public AdminRepository(AlamutDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Advertisement>> AllUnPublished(AdvertisementParameters advertisementParameters) 
                => await PaginatedList<Advertisement>
                .CreateAsync(_context.Advertisements.AsNoTracking()
                    .Where(x => x.Published == false)
                    .OrderBy(x => x.DatePosted),
                advertisementParameters.PageNumber,
                advertisementParameters.PageSize);

        public async Task<Advertisement> Publish(int id)
        {
            var advertisement = await _context.Advertisements.FindAsync(id);
            if (advertisement == null) return null;
            advertisement.Published = true;
            return advertisement;
        }

        public async new Task<Advertisement> Delete(int id)
        {
            var advertisement = await _context.Advertisements.FindAsync(id);
            if (advertisement == null) return null;
            _context.Advertisements.Remove(advertisement);
            return advertisement;
        }

        public async Task<PaginatedList<Advertisement>> GetUserAds(string userId, AdvertisementParameters advertisementParameters)
            => await PaginatedList<Advertisement>
            .CreateAsync(_context.Advertisements.AsNoTracking().Where(x => userId == x.UserId.ToString() && x.Published == true),
            advertisementParameters.PageNumber,
            advertisementParameters.PageSize);

        public async Task<PaginatedList<Advertisement>> GetReportedAds(AdvertisementParameters advertisementParameters)
                    => await PaginatedList<Advertisement>
                    .CreateAsync(_context.Advertisements.AsNoTracking().Where(x => x.ReportMessage != null).OrderBy(x => x.DatePosted),
                    advertisementParameters.PageNumber,
                    advertisementParameters.PageSize);

        public async Task CleanReport(int id)
        {
            var result = await _context.Advertisements.FindAsync(id);
            if (result != null) result.ReportMessage = null;
        }
    }
}
