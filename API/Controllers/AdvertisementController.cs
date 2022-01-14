using application.DTOs;
using application.DTOs.Advertisement;
using application.Interfaces.repository;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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


        [HttpGet("getUnpublished")]
        public async Task<IEnumerable<AdvertisementDto>> GetAllUnPublished()
        {


            var result = await _advertisementRepository.GetAllUnpublished();
            return  result.Select(x => new AdvertisementDto 
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
                Village = x.Village,

            });
         
          
        }

        [HttpGet("getUnpublishedUserAds/{userid}")]
        public async Task<IEnumerable<AdvertisementDto>> GetUnpublishedUserAds(string userId)
        {


            var result = await _advertisementRepository.GetUnpublishedUserAds(userId);
            return result.Select(x => new AdvertisementDto
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
                Village = x.Village,

            });


        }

        [HttpGet("filter/{adstype}")]
        public async Task<IEnumerable<AdvertisementDto>> GetAll([FromQuery] AdvertisementParameters advertisementParameters, string? adstype)
        {
            IEnumerable<AdvertisementDto> result;
            object metadata;
            if (string.IsNullOrWhiteSpace(adstype))
            {
                var allAdvertisement = await _advertisementRepository.GetAll(advertisementParameters);

                 metadata = new
                {
                    allAdvertisement.TotalCount,
                    allAdvertisement.PageSize,
                    allAdvertisement.CurrentPage,
                    allAdvertisement.TotalPages,
                    allAdvertisement.HasNext,
                    allAdvertisement.HasPrevious
                };

                result = allAdvertisement.Select(x => new AdvertisementDto
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
                    Village = x.Village,
                });
            }
            else
            {
                var filteredResult = await _advertisementRepository.GetAll(adstype, advertisementParameters);

                metadata = new
                {
                    filteredResult.TotalCount,
                    filteredResult.PageSize,
                    filteredResult.CurrentPage,
                    filteredResult.TotalPages,
                    filteredResult.HasNext,
                    filteredResult.HasPrevious
                };

                result = filteredResult.Select(x => new AdvertisementDto
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
                    Village = x.Village,
                });
            }

          
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return result;
         
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
                    Village = x.Village,
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
            var myAds = await _advertisementRepository.GetCurrentUserAds(currentuser);
            return myAds.Select(x => new AdvertisementDto
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
                Village = x.Village,
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
