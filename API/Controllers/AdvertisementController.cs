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
                adsType = x.AdsType,
                area = x.Area,
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
                    adsType = x.AdsType,
                    area = x.Area,
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
                 adsType = x.AdsType,
                 area = x.Area,
                 UserId = x.UserId,
             });
        }
        [HttpGet("find/{input}")]
        public async Task<IEnumerable<AdvertisementDto>> Search(string input)
        {
            return _advertisementRepository.Find(input).Result.Select(x => new AdvertisementDto
            {
                Title = x.Title,
                Id = x.Id,
                Price = x.Price,
                Photo1 = x.photo1,
                Photo2 = x.photo2,
                Description = x.Description,
                DatePosted = x.DatePosted,
                DaySended = x.DatePosted.ToString(),
                adsType = x.AdsType,
                area = x.Area,
                UserId = x.UserId,
            });
        }


        [HttpGet("{id}")]
        public async Task<Advertisement> Get(int id)
        {
            return await _advertisementRepository.Get(id) ;
        }

        [HttpGet("myalamuti/myAds")]
        public async Task<IEnumerable<AdvertisementDto>> Get()
        {
            var userId = User.Claims.FirstOrDefault().Value;
            var currentuser = await _userManager.FindByIdAsync(userId);
            return  _advertisementRepository.GetCurrentUserAds(currentuser).Result.Select(x => new AdvertisementDto
            {
                Title = x.Title,
                Id = x.Id,
                Price = x.Price,
                Photo1 = x.photo1,
                Photo2 = x.photo2,
                Description = x.Description,
                DatePosted = x.DatePosted,
                DaySended = x.DatePosted.ToString(),
                adsType = x.AdsType,
                area = x.Area,
                UserId = x.UserId,



            });
        }

        [HttpPost]
        public  async Task<Advertisement> Post([FromForm] Advertisement  advertisement)
        {
            var userId = User.Claims.FirstOrDefault().Value;
            var currentuser = await _userManager.FindByIdAsync(userId);
            //8c2b9c88 - 3c8b - 4d41 - b7dc - 399c8e66a643
            //var username2 = User.Claims.FirstOrDefault();
            //var userId = User.FindFirstValue(ClaimTypes.Name);
            //var userId2 = this.User.FindFirstValue(ClaimTypes.Uri);
            //var userId3 = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userId4 = this.User.FindFirstValue(ClaimTypes.Name);

            //var user = await _userManager.FindByNameAsync(userId);
            advertisement.UserId = currentuser.Id;


            //foreach (var file in Request.Form.Files)
            //{
            //    Image img = new Image();
            //    img.ImageTitle = file.FileName;
            //    MemoryStream ms = new MemoryStream();
            //    file.CopyTo(ms);
            //    if (advertisement.photo1 == null)
            //    {
            //        advertisement.photo1 = ms.ToArray();
            //    }
            //    if (advertisement.photo2== null)
            //    {
            //        advertisement.photo2 = ms.ToArray();
            //    }
            //    ms.Close();
            //    ms.Dispose();
            //}
            return await _advertisementRepository.Add(advertisement);
        }


        [HttpPut]
        public async Task<IActionResult> Put([FromForm] Advertisement advertisement)
        {
     
            var userId = User.Claims.FirstOrDefault().Value;
            var currentuser = await _userManager.FindByIdAsync(userId);
            var ads = await _advertisementRepository.Get(advertisement.Id);
            if (ads.UserId == currentuser.Id)
            {
                _advertisementRepository.Update(advertisement);
                return Ok(advertisement);
            }
            return NotFound();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.Claims.FirstOrDefault().Value;
            var currentuser = await _userManager.FindByIdAsync(userId);
            var ads = await _advertisementRepository.Get(id);
            if (ads.UserId == currentuser.Id)
            {
                await _advertisementRepository.Delete(ads);
                return Ok();
            }
            return NotFound();
            
        }
    }
}
