using application.DTOs;
using application.DTOs.Advertisement;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.Interfaces.repository
{
    public interface IAdvertisementRepository : IRepository<Advertisement>
    {
        Task<PaginatedList<Advertisement>> GetCurrentUserAds(IdentityUser user, AdvertisementParameters advertisementParameters);
        Task<PaginatedList<Advertisement>> Search(string input, AdvertisementParameters advertisementParameters);
        Task<PaginatedList<Advertisement>> GetAll(string adstype, AdvertisementParameters advertisementParameters);
        Task<PaginatedList<Advertisement>> GetAll(AdvertisementParameters advertisementParameters);
        Task<PaginatedList<Advertisement>> GetAllUnpublished(AdvertisementParameters advertisementParameters);
        Task<Advertisement> ChangeToPublished(int id);
        Task<Advertisement> DeleteUnpublished(int id);
        Task<PaginatedList<Advertisement>> GetUnpublishedUserAds(string userId, AdvertisementParameters advertisementParameters);
        Task<Advertisement> ReportAdvertisement(int id,string message);
        Task<Advertisement> RemoveReportAdvertisement(int id);
        Task<PaginatedList<Advertisement>> GetReportedAdvertisements(AdvertisementParameters advertisementParameters);
    }
}
