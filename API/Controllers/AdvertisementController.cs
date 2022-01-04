using application.DTOs;
using application.Interfaces.Data;
using application.Interfaces.repository;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AdvertisementController : ControllerBase
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
     

      

        public AdvertisementController(IAdvertisementRepository advertisementRepository,IHttpContextAccessor httpContextAccessor,UserManager<IdentityUser> userManager)
        {
            _advertisementRepository = advertisementRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager; 
        }

        [HttpGet()]
        public async Task<IEnumerable<AdvertisementDto>> GetAll()
        {
   

            return _advertisementRepository.GetAll().Result.Select(x => new AdvertisementDto
            {
                Title = x.Title,
                Id = x.Id,
                Price = x.Price,
                Photo1 = x.photo1,
                Photo2 = x.photo2,
                Description = x.Description,
                DatePosted = x.DatePosted,
                DaySended = x.DatePosted.ToString(),
                AdsType = x.AdsType,
                PhoneNumber = _userManager.FindByIdAsync(x.UserId).Result.UserName,
                Published = x.Published,
                Area = x.Area,
                UserId = x.UserId,
                
            });
        }

        [HttpGet("getUnpublished")]
        public async Task<IEnumerable<AdvertisementDto>> GetAllUnPublished()
        {

         
                return  _advertisementRepository.GetAllUnpublished().Result.Select(x => new AdvertisementDto
                {
                    Title = x.Title,
                    Id = x.Id,
                    Price = x.Price,
                    Photo1 = x.photo1,
                    Photo2 = x.photo2,
                    Description = x.Description,
                    DatePosted = x.DatePosted,
                    DaySended = x.DatePosted.ToString(),
                    PhoneNumber = _userManager.FindByIdAsync(x.UserId).Result.UserName,
                    Published = x.Published,
                    AdsType = x.AdsType,
                    Area = x.Area,
                    UserId = x.UserId,

                });
         
          
        }

        [HttpGet("getUnpublishedUserAds/{userid}")]
        public async Task<IEnumerable<AdvertisementDto>> GetUnpublishedUserAds(string userId)
        {


            return _advertisementRepository.GetUnpublishedUserAds(userId).Result.Select(x => new AdvertisementDto
            {
                Title = x.Title,
                Id = x.Id,
                Price = x.Price,
                Photo1 = x.photo1,
                Photo2 = x.photo2,
                Description = x.Description,
                DatePosted = x.DatePosted,
                DaySended = x.DatePosted.ToString(),
                AdsType = x.AdsType,
                PhoneNumber = _userManager.FindByIdAsync(x.UserId).Result.UserName,
                Published = x.Published,
                Area = x.Area,
                UserId = x.UserId,

            });


        }

        [HttpGet("filter/{adstype}")]
        public async Task<IEnumerable<AdvertisementDto>> Get(string? adstype)
        {
   
            if (adstype != null)
            {
                return _advertisementRepository.GetAll(adstype).Result.Select(x => new AdvertisementDto
                {
                    Title = x.Title,
                    Id = x.Id,
                    Price = x.Price,
                    Photo1 = x.photo1,
                    Photo2 = x.photo2,
                    Description = x.Description,
                    DatePosted = x.DatePosted,
                    DaySended = x.DatePosted.ToString(),
                    AdsType = x.AdsType,
                    PhoneNumber = _userManager.FindByIdAsync(x.UserId).Result.UserName,
                    Published = x.Published,
                    Area = x.Area,
                    UserId = x.UserId,
                });
            }
             return  _advertisementRepository.GetAll().Result.Select(x=> new AdvertisementDto { 
                 Title = x.Title,Id = x.Id,
                 Price= x.Price,
                 Photo1=x.photo1,
                 Photo2=x.photo2,
                 Description=x.Description,
                 DatePosted=x.DatePosted,
                 DaySended = x.DatePosted.ToString(),
                 AdsType = x.AdsType,
                 PhoneNumber = _userManager.FindByIdAsync(x.UserId).Result.UserName,
                 Published = x.Published,
                 Area = x.Area,
                 UserId = x.UserId,
             });
        }
        [HttpGet("search/{input}")]
        public async Task<IActionResult> Search(string input)
        {
            var searchResult = await _advertisementRepository.Search(input);

         
                return Ok(searchResult.Select(x => new AdvertisementDto
                {
                    Title = x.Title,
                    Id = x.Id,
                    Price = x.Price,
                    Photo1 = x.photo1,
                    Photo2 = x.photo2,
                    Description = x.Description,
                    DatePosted = x.DatePosted,
                    DaySended = x.DatePosted.ToString(),
                    AdsType = x.AdsType,
                    Area = x.Area,
                    PhoneNumber = _userManager.FindByIdAsync(x.UserId).Result.UserName,
                    Published = x.Published,
                    UserId = x.UserId,
                }));
       

          
           
        }


        [HttpGet("{id}")]
        public async Task<Advertisement> Get(int id) => await _advertisementRepository.Get(id);


        [HttpPut("changeToPublished/{id}")]
        public async Task<Advertisement> ChangeToPublished(int id) => await _advertisementRepository.ChangeToPublished(id);


        [HttpDelete("unPublished/{id}")]
        public async Task<Advertisement> DeleteUnpublished(int id) => await _advertisementRepository.DeleteUnpublished(id);


        [HttpGet("myalamuti/myAds")]
        public async Task<IEnumerable<AdvertisementDto>> Get()
        {
            var currentuser = await _userManager.FindByIdAsync(User.Claims.FirstOrDefault()?.Value);
            return _advertisementRepository.GetCurrentUserAds(currentuser).Result.Select(x => new AdvertisementDto
            {
                Title = x.Title,
                Id = x.Id,
                Price = x.Price,
                Photo1 = x.photo1,
                Photo2 = x.photo2,
                Description = x.Description,
                DatePosted = x.DatePosted,
                DaySended = x.DatePosted.ToString(),
                AdsType = x.AdsType,
                Area = x.Area,
                PhoneNumber = _userManager.FindByIdAsync(x.UserId).Result.UserName,
                Published = x.Published,
                UserId = x.UserId,
            });
        }

        [HttpPost]
        public async Task<Advertisement> Post([FromForm] Advertisement advertisement)
        {
            var userId = User.Claims.FirstOrDefault()?.Value;

            advertisement.UserId = userId;

            return await _advertisementRepository.Add(advertisement);
        }


        [HttpPut]
        public async Task<IActionResult> Put([FromForm] Advertisement advertisement)
        {

            var userId = User.Claims.FirstOrDefault()?.Value;
            var ads = await _advertisementRepository.Get(advertisement.Id);

            if (ads.UserId != userId)
            {
                return NotFound();
            }
            await _advertisementRepository.Update(advertisement);
            return Ok(advertisement);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.FirstOrDefault()?.Value;
            //var currentuser = await _userManager.FindByIdAsync(userId);
            var ads = await _advertisementRepository.Get(id);
            if (ads.UserId == userId)
            {
                await _advertisementRepository.Delete(ads);
                return Ok(ads);
            }
            return NotFound();
            
        }
    }
}
