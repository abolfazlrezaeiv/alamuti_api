using application.DTOs;
using application.DTOs.Advertisement;
using application.Interfaces.repository;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AdvertisementController : ControllerBase
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;

        public AdvertisementController(IAdvertisementRepository advertisementRepository, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            _advertisementRepository = advertisementRepository;
            _userManager = userManager;
            _mapper = mapper;
        }


        [HttpGet("getUnpublished")]
        public IEnumerable<AdvertisementDto> GetAllUnPublished([FromQuery] AdvertisementParameters advertisementParameters)
        {


            var result = _advertisementRepository.GetAllUnpublished(advertisementParameters);

            var metadata = new
            {
                result.TotalCount,
                result.PageSize,
                result.CurrentPage,
                result.TotalPages,
                result.HasNext,
                result.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return result.Select(x => _mapper.Map<AdvertisementDto>(x));


        }

        [HttpGet("GetUserAdvertisementInAdminPandel/{userid}")]
        public IEnumerable<AdvertisementDetailDto> GetUserAdvertisementInAdminPandel(string userId
            , [FromQuery] AdvertisementParameters advertisementParameters)
        {
            var result = _advertisementRepository.GetUnpublishedUserAds(userId, advertisementParameters);

            var metadata = new
            {
                result.TotalCount,
                result.PageSize,
                result.CurrentPage,
                result.TotalPages,
                result.HasNext,
                result.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return result.Select(x => _mapper.Map<AdvertisementDetailDto>(x));


        }

        [HttpGet("filter/{adstype}")]
        public IEnumerable<AdvertisementDto> GetAll([FromQuery] AdvertisementParameters advertisementParameters, string? adstype)
        {
            object metadata;
            if (string.IsNullOrWhiteSpace(adstype))
            {
                var allAdvertisement = _advertisementRepository.GetAll(advertisementParameters);

                metadata = new
                {
                    allAdvertisement.TotalCount,
                    allAdvertisement.PageSize,
                    allAdvertisement.CurrentPage,
                    allAdvertisement.TotalPages,
                    allAdvertisement.HasNext,
                    allAdvertisement.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                return allAdvertisement.Select(x => _mapper.Map<AdvertisementDto>(x));
            }
            else
            {
                var filteredResult = _advertisementRepository.GetAll(adstype, advertisementParameters);

                metadata = new
                {
                    filteredResult.TotalCount,
                    filteredResult.PageSize,
                    filteredResult.CurrentPage,
                    filteredResult.TotalPages,
                    filteredResult.HasNext,
                    filteredResult.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                return filteredResult.Select(x => _mapper.Map<AdvertisementDto>(x));
            }




        }
        [HttpGet("search/{input}")]
        public IActionResult Search(string input, [FromQuery] AdvertisementParameters advertisementParameters)
        {
            var searchResult = _advertisementRepository.Search(input, advertisementParameters);

            var metadata = new
            {
                searchResult.TotalCount,
                searchResult.PageSize,
                searchResult.CurrentPage,
                searchResult.TotalPages,
                searchResult.HasNext,
                searchResult.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(searchResult.Select(x => _mapper.Map<AdvertisementDto>(x)));
        }


        [HttpGet("{id}")]
        public async Task<AdvertisementDetailDto> Get(int id)
        {
            var result = await _advertisementRepository.Get(id);
            return _mapper.Map<AdvertisementDetailDto>(result);
        }


        [HttpPut("changeToPublished/{id}")]
        public async Task<Advertisement> ChangeToPublished(int id) => await _advertisementRepository.ChangeToPublished(id);


        [HttpDelete("unPublished/{id}")]
        public async Task<Advertisement> DeleteUnpublished(int id) => await _advertisementRepository.DeleteUnpublished(id);


        [HttpGet("useradvertisement")]
        public async Task<IEnumerable<UserAdvertisementDto>> Get([FromQuery] AdvertisementParameters advertisementParameters)
        {
            var currentuser = await _userManager.FindByIdAsync(User.Claims.FirstOrDefault()?.Value);
            var userAds = _advertisementRepository.GetCurrentUserAds(currentuser, advertisementParameters);

            var metadata = new
            {
                userAds.TotalCount,
                userAds.PageSize,
                userAds.CurrentPage,
                userAds.TotalPages,
                userAds.HasNext,
                userAds.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return userAds.Select(x => _mapper.Map<UserAdvertisementDto>(x));
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
