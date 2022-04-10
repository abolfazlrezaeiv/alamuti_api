using Alamuti.Domain.Entities;
using application.DTOs.Advertisement;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace application.AutoMapper
{
    public class MapPhoneNumber : IMappingAction<Advertisement, AdvertisementDetailDto>
    {
        private readonly UserManager<AlamutiUser> _userManager;

        public MapPhoneNumber(UserManager<AlamutiUser> userManager)
        {
            _userManager = userManager;
        }


        public void Process(Advertisement source, AdvertisementDetailDto destination, ResolutionContext context)
        {
            var phoneNumber =  _userManager.FindByIdAsync(source.UserId).Result.UserName;
            if (phoneNumber == null)
            {
                return;
            }
            destination.PhoneNumber = phoneNumber;
        }

       
    }
}