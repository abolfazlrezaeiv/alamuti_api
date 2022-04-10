using application.DTOs;
using application.DTOs.Advertisement;
using application.Interfaces.repository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamuti.Application.Interfaces.repository
{
    public interface IAdminRepository : IGenericRepository<Advertisement>
    {
        Task<PaginatedList<Advertisement>> AllUnPublished(AdvertisementParameters advertisementParameters);
        Task<PaginatedList<Advertisement>> GetUserAds(string userId, AdvertisementParameters advertisementParameters);
        Task<PaginatedList<Advertisement>> GetReportedAds(AdvertisementParameters advertisementParameters);
        Task<Advertisement> Publish(int id);      
        Task CleanReport(int id);
        new Task<Advertisement> Delete(int id); 
    }
}
