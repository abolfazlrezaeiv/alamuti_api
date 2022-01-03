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
        Task<IEnumerable<Advertisement>> GetCurrentUserAds(IdentityUser user);
        Task<IEnumerable<Advertisement>> Find(string input);
        Task<IEnumerable<Advertisement>> GetAll(string adstype);
        Task<IEnumerable<Advertisement>> GetAllUnpublished();
        Task<Advertisement> ChangeToPublished(int id);
        Task<Advertisement> DeleteUnpublished(int id);
        Task<IEnumerable<Advertisement>> GetUnpublishedUserAds(string userId);



    }
}
