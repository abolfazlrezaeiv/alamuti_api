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
        PagedList<Advertisement> GetCurrentUserAds(IdentityUser user, AdvertisementParameters advertisementParameters);
        PagedList<Advertisement> Search(string input, AdvertisementParameters advertisementParameters);
        PagedList<Advertisement> GetAll(string adstype, AdvertisementParameters advertisementParameters);
        PagedList<Advertisement> GetAll(AdvertisementParameters advertisementParameters);
        PagedList<Advertisement> GetAllUnpublished(AdvertisementParameters advertisementParameters);
        Task<Advertisement> ChangeToPublished(int id);
        Task<Advertisement> DeleteUnpublished(int id);
        PagedList<Advertisement> GetUnpublishedUserAds(string userId, AdvertisementParameters advertisementParameters);



    }
}
