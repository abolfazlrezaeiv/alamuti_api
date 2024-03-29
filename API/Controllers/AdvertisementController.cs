﻿using Alamuti.Application.Interfaces.UnitOfWork;
using Alamuti.Domain.Entities;
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

    [Route("api/advertisements")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AdvertisementController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AlamutiUser> _userManager;


        public AdvertisementController(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            UserManager<AlamutiUser> userManager
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }


        [AllowAnonymous]
        [HttpGet("all/{category}")]
        public async Task<IEnumerable<AdvertisementDto>> GetAll([FromQuery] AdvertisementParameters advertisementParameters, string? category)
        {
            PaginatedList<Advertisement> advertisements;
            if (string.IsNullOrWhiteSpace(category)) advertisements = await _unitOfWork.Advertisement.All(advertisementParameters);
            else advertisements = await _unitOfWork.Advertisement.FilterByType(category, advertisementParameters);
            AddHeaderPagination(advertisements);
            return advertisements.Select(x => _mapper.Map<AdvertisementDto>(x));
        }


        [HttpGet("user-advertisements")]
        public async Task<IEnumerable<UserAdvertisementDto>> Get([FromQuery] AdvertisementParameters advertisementParameters)
        {
            var currentuser = await _userManager.FindByIdAsync(User.Claims.FirstOrDefault()?.Value);
            var userAds = await _unitOfWork.Advertisement.GetUserAds(currentuser, advertisementParameters);
            AddHeaderPagination(userAds);
            return userAds.Select(x => _mapper.Map<UserAdvertisementDto>(x));
        }


        [AllowAnonymous]
        [HttpGet("search/{keyword}")]
        public async Task<IEnumerable<AdvertisementDto>> Search(string keyword, [FromQuery] AdvertisementParameters advertisementParameters)
        {
            var searchResult = await _unitOfWork.Advertisement.Search(keyword, advertisementParameters);
            AddHeaderPagination(searchResult);
            return searchResult.Select(x => _mapper.Map<AdvertisementDto>(x));
        }


        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<AdvertisementDetailDto> Details(int id) => _mapper.Map<AdvertisementDetailDto>(await _unitOfWork.Advertisement.GetById(id));

        [AllowAnonymous]
        [HttpPut("reports")]
        public async Task InsertReport([FromForm] int id, [FromForm] string message)
        {
            await _unitOfWork.Advertisement.InsertReport(id, message);
            await _unitOfWork.CompleteAsync();
        }


        [HttpPost]
        public async Task Post([FromForm] Advertisement advertisement)
        {
            var userId = User.Claims.FirstOrDefault()?.Value;
            advertisement.UserId = userId;
            await _unitOfWork.Advertisement.Add(advertisement);
            await _unitOfWork.CompleteAsync();
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromForm] Advertisement advertisement)
        {
            var userId = User.Claims.FirstOrDefault()?.Value;
            var result = await _unitOfWork.Advertisement.Update(userId, advertisement);
            if (result == false) return NotFound();
            await _unitOfWork.CompleteAsync();
            return Ok();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.FirstOrDefault()?.Value;
            var result = await _unitOfWork.Advertisement.Delete(id,userId);
            if (result == false) return NotFound();
            await _unitOfWork.CompleteAsync();
            return Ok(result);
        }
    }
