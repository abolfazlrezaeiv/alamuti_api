using Alamuti.Domain.Entities;
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
    public interface IAdvertisementRepository : IGenericRepository<Advertisement>
    {
        Task<PaginatedList<Advertisement>> GetUserAds(AlamutiUser user, AdvertisementParameters advertisementParameters);
        Task<PaginatedList<Advertisement>> Search(string input, AdvertisementParameters advertisementParameters);
        Task<PaginatedList<Advertisement>> FilterByType(string adstype, AdvertisementParameters advertisementParameters);
        Task<PaginatedList<Advertisement>> All(AdvertisementParameters advertisementParameters);
        Task<bool> Update(string userId, Advertisement updatedData);
        Task<bool> InsertReport(int id,string message);
        Task<bool> Delete(int adsId, string userId);
    }
}
