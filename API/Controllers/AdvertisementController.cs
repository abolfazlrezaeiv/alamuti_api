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
        public async Task<IEnumerable<AdvertisementDto>> GetAllUnPublished([FromQuery] AdvertisementParameters advertisementParameters)
        {


            var result = await _advertisementRepository.GetAllUnpublished(advertisementParameters);

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

        [HttpGet("adminpaneluseradvertisement")]
        public async Task<IEnumerable<AdvertisementDto>> GetUserAdvertisementInAdminPandel([FromQuery]string userId,[FromQuery] AdvertisementParameters advertisementParameters
            )
        {
            var result = await _advertisementRepository.GetUnpublishedUserAds(userId, advertisementParameters);

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

        [HttpGet("filter/{adstype}")]
        public async Task<IEnumerable<AdvertisementDto>> GetAll([FromQuery] AdvertisementParameters advertisementParameters, string? adstype)
        {

            if (string.IsNullOrWhiteSpace(adstype))
            {
                var allAdvertisement = await _advertisementRepository.GetAll(advertisementParameters);

                var metadata = new
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
                var filteredResult = await _advertisementRepository.GetByFilter(adstype, advertisementParameters);

                var metadata = new
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
        public async Task<IEnumerable<AdvertisementDto>> Search(string input, [FromQuery] AdvertisementParameters advertisementParameters)
        {
            var searchResult = await _advertisementRepository.Search(input, advertisementParameters);

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

            return searchResult.Select(x => _mapper.Map<AdvertisementDto>(x));
        }


        [HttpGet("{id}")]
        public async Task<AdvertisementDetailDto> Get(int id)
        {
            var result = await _advertisementRepository.Get(id);
            return _mapper.Map<AdvertisementDetailDto>(result);
        }


        [HttpPut("changeToPublished/{id}")]
        public async Task ChangeToPublished(int id) => await _advertisementRepository.ChangeToPublished(id);

        [HttpPut("report")]
        public async Task ReportAdvertisement([FromForm] int id, [FromForm] string message) => await _advertisementRepository.ReportAdvertisement(id,message);


        [HttpPut("report/{id}")]
        public async Task RemoveReportAdvertisement(int id) => await _advertisementRepository.RemoveReportAdvertisement(id);

        [HttpGet("report")]
        public async Task<IEnumerable<AdvertisementDetailDto>> GetReports([FromQuery]AdvertisementParameters advertisementParameters)
        {
          
            var reports = await _advertisementRepository.GetReportedAdvertisements(advertisementParameters);

            var metadata = new
            {
                reports.TotalCount,
                reports.PageSize,
                reports.CurrentPage,
                reports.TotalPages,
                reports.HasNext,
                reports.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return reports.Select(x => _mapper.Map<AdvertisementDetailDto>(x));
        }


        [HttpDelete("unPublished/{id}")]
        public async Task DeleteUnpublished(int id) => await _advertisementRepository.DeleteUnpublished(id);


        [HttpGet("useradvertisement")]
        public async Task<IEnumerable<UserAdvertisementDto>> Get([FromQuery] AdvertisementParameters advertisementParameters)
        {
            var currentuser = await _userManager.FindByIdAsync(User.Claims.FirstOrDefault()?.Value);
            var userAds = await _advertisementRepository.GetCurrentUserAds(currentuser, advertisementParameters);

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
        public async Task Post([FromForm] Advertisement advertisement)
        {
            var userId = User.Claims.FirstOrDefault()?.Value;

            advertisement.UserId = userId;

            await _advertisementRepository.Add(advertisement);
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
