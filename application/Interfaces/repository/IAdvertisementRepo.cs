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
        Task<PaginatedList<Advertisement>> GetByFilter(string adstype, AdvertisementParameters advertisementParameters);
        Task<PaginatedList<Advertisement>> GetAll(AdvertisementParameters advertisementParameters);
        Task<PaginatedList<Advertisement>> GetAllUnpublished(AdvertisementParameters advertisementParameters);
        Task ChangeToPublished(int id);
        Task DeleteUnpublished(int id);
        Task<PaginatedList<Advertisement>> GetUnpublishedUserAds(string userId, AdvertisementParameters advertisementParameters);
        Task ReportAdvertisement(int id,string message);
        Task RemoveReportAdvertisement(int id);
        Task<PaginatedList<Advertisement>> GetReportedAdvertisements(AdvertisementParameters advertisementParameters);
    }
}
