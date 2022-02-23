using application.DTOs;
using application.DTOs.Advertisement;
using application.Interfaces.repository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AdminController : BaseController
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IMapper _mapper;

        public AdminController(IAdvertisementRepository advertisementRepository, IMapper mapper)
        {
            _advertisementRepository = advertisementRepository;
            _mapper = mapper;
        }


        [HttpPut("advertisements/{id}")]
        public async Task Publish(int id) => await _advertisementRepository.ChangeToPublished(id);


        [HttpPut("reports/{id}")]
        public async Task RemoveReport(int advertisementId) => await _advertisementRepository.RemoveReportAdvertisement(advertisementId);


        [HttpGet("reports")]
        public async Task<IEnumerable<AdvertisementDetailDto>> GetReports([FromQuery] AdvertisementParameters advertisementParameters)
        {
            var reports = await _advertisementRepository.GetReportedAdvertisements(advertisementParameters);

            AddHeaderPagination(reports);

            return reports.Select(x => _mapper.Map<AdvertisementDetailDto>(x));
        }

        [HttpDelete("advertisements/{id}")]
        public async Task DeleteUnpublished(int id) => await _advertisementRepository.DeleteUnpublished(id);
  

        [HttpGet("user-advertisements")]
        public async Task<IEnumerable<AdvertisementDto>> GetUserAds(
          [FromQuery] string userId,
          [FromQuery] AdvertisementParameters advertisementParameters)
        {
            var result = await _advertisementRepository.GetUnpublishedUserAds(userId, advertisementParameters);

            AddHeaderPagination(result);

            return result.Select(x => _mapper.Map<AdvertisementDto>(x));
        }


        [HttpGet("unpublished-advertisements")]
        public async Task<IEnumerable<AdvertisementDto>> GetAllUnPublished([FromQuery] AdvertisementParameters advertisementParameters)
        {
            var result = await _advertisementRepository.GetAllUnpublished(advertisementParameters);

            AddHeaderPagination(result);

            return result.Select(x => _mapper.Map<AdvertisementDto>(x));
        }
    }
}
