using application.DTOs.Advertisement;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

public class MapPhoneNumber : IMappingAction<Advertisement, AdvertisementDetailDto>
{
    private readonly UserManager<IdentityUser> _userManager;

    public MapPhoneNumber(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }


    public void Process(Advertisement source, AdvertisementDetailDto destination, ResolutionContext context)
    {
        destination.PhoneNumber = _userManager.FindByIdAsync(source.UserId).Result.UserName;
    }
}