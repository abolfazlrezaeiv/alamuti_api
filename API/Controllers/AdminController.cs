using Alamuti.Application.Interfaces.UnitOfWork;
using application.DTOs;
using application.DTOs.Advertisement;
using application.Interfaces.repository;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

    [Authorize(Policy = "AdminOnly")]
    [Route("api/admin")]
    [ApiController]
    public class AdminController : BaseController
    {
        private readonly IUnitOfWork _unitOfWord;
        private readonly IMapper _mapper;

        public AdminController(IUnitOfWork unitOfWord, IMapper mapper)
        {
            _unitOfWord = unitOfWord;
            _mapper = mapper;
        }


        [HttpPut("advertisements/{id}")]
        public async Task<IActionResult> Publish(int id) 
        { 
            var result = await _unitOfWord.Admin.Publish(id);
            if (result == null) return NotFound();
            await _unitOfWord.Chat.AddAlamutiChat(result, PublishStatus.accept);
            await _unitOfWord.CompleteAsync();
            return Ok(result);
        }


        [HttpPut("reports/{id}")]
        public async Task RemoveReport(int advertisementId) 
        { 
            await _unitOfWord.Admin.CleanReport(advertisementId); 
            await _unitOfWord.CompleteAsync();
        }


        [HttpGet("advertisements/reports")]
        public async Task<IEnumerable<AdvertisementDetailDto>> GetReports([FromQuery] AdvertisementParameters advertisementParameters)
        {
            var reports = await _unitOfWord.Admin.GetReportedAds(advertisementParameters);
            AddHeaderPagination(reports);
            return reports.Select(x => _mapper.Map<AdvertisementDetailDto>(x));
        }

        [HttpDelete("advertisements/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _unitOfWord.Admin.Delete(id);
            if (result is null) return NotFound();
            await _unitOfWord.Chat.AddAlamutiChat(result, PublishStatus.reject);
            await _unitOfWord.CompleteAsync();
            return Ok();
        }
  

        [HttpGet("user-advertisements")]
        public async Task<IEnumerable<AdvertisementDto>> GetUserAds(
          [FromQuery] string userId,
          [FromQuery] AdvertisementParameters advertisementParameters)
        {
            var result = await _unitOfWord.Admin.GetUserAds(userId, advertisementParameters);
            AddHeaderPagination(result);
            return result.Select(x => _mapper.Map<AdvertisementDto>(x));
        }

        [AllowAnonymous]
        [HttpGet("advertisements")]
        public async Task<IActionResult> GetAllUnPublished([FromQuery] AdvertisementParameters advertisementParameters)
        {
                var result = await _unitOfWord.Admin.AllUnPublished(advertisementParameters);
                AddHeaderPagination(result);
                return Ok(result.Select(x => _mapper.Map<AdvertisementDto>(x)));
        }
    }
