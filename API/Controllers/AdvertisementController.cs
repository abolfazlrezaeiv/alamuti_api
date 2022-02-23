using application.DTOs;
using application.DTOs.Advertisement;
using application.Interfaces.repository;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [Route("api/advertisement")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AdvertisementController : BaseController
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;


        public AdvertisementController(IAdvertisementRepository advertisementRepository, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _advertisementRepository = advertisementRepository;
            _mapper = mapper;
            _userManager = userManager;


        }


        [AllowAnonymous]
        [HttpGet("index/{category}")]
        public async Task<IEnumerable<AdvertisementDto>> Index([FromQuery] AdvertisementParameters advertisementParameters, string? category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                var allAdvertisement = await _advertisementRepository.GetAll(advertisementParameters);

                AddHeaderPagination(allAdvertisement);

                return allAdvertisement.Select(x => _mapper.Map<AdvertisementDto>(x));
            }
            else
            {
                var filteredResult = await _advertisementRepository.GetByFilter(category, advertisementParameters);

                AddHeaderPagination(filteredResult);

                return filteredResult.Select(x => _mapper.Map<AdvertisementDto>(x));
            }
        }


        [HttpGet("user-advertisements")]
        public async Task<IEnumerable<UserAdvertisementDto>> Get([FromQuery] AdvertisementParameters advertisementParameters)
        {
            var currentuser = await _userManager.FindByIdAsync(User.Claims.FirstOrDefault()?.Value);
            var userAds = await _advertisementRepository.GetCurrentUserAds(currentuser, advertisementParameters);

            AddHeaderPagination(userAds);

            return userAds.Select(x => _mapper.Map<UserAdvertisementDto>(x));
        }


        [AllowAnonymous]
        [HttpGet("search/{keyword}")]
        public async Task<IEnumerable<AdvertisementDto>> Search(string keyword, [FromQuery] AdvertisementParameters advertisementParameters)
        {
            var searchResult = await _advertisementRepository.Search(keyword, advertisementParameters);

            AddHeaderPagination(searchResult);

            return searchResult.Select(x => _mapper.Map<AdvertisementDto>(x));
        }


        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<AdvertisementDetailDto> Details(int id)
        {
            var result = await _advertisementRepository.Get(id);
            return _mapper.Map<AdvertisementDetailDto>(result);
        }


        [HttpPut("reports")]
        public async Task InsertReport([FromForm] int id, [FromForm] string message)
        {
            await _advertisementRepository.ReportAdvertisement(id, message);
        }


        [HttpPost]
        public async Task Post([FromForm] Advertisement advertisement)
        {
            var userId = User.Claims.FirstOrDefault()?.Value;

            advertisement.UserId = userId;

            await _advertisementRepository.Add(advertisement);
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromForm] Advertisement advertisement)
        {
            var userId = User.Claims.FirstOrDefault()?.Value;
            var ads = await _advertisementRepository.Get(advertisement.Id);

            if (ads.UserId != userId)
            {
                return NotFound();
            }
            await _advertisementRepository.Update(advertisement);
            return Ok();
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
