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
        private readonly IRepository<Advertisement> _advertisementRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
     

      

        public AdvertisementController(IRepository<Advertisement> advertisementRepository,IHttpContextAccessor httpContextAccessor,UserManager<IdentityUser> userManager)
        {
            _advertisementRepository = advertisementRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager; 
        }


        [HttpGet]
        public async Task<IEnumerable<AdvertisementDto>> Get() 
        { 
             return  _advertisementRepository.GetAll().Result.Select(x=> new AdvertisementDto { 
                 Title = x.Title,Id = x.Id,
                 Price= x.Price,
                 Photo=x.photo,
                 Description=x.Description,
                 DatePosted=x.DatePosted,
                 DaySended = x.DatePosted.ToString()
             });
        }


        [HttpGet("{id}")]
        public async Task<Advertisement> Get(int id)
        {
            return await _advertisementRepository.Get(id) ;
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


            foreach (var file in Request.Form.Files)
            {
                Image img = new Image();
                img.ImageTitle = file.FileName;
                MemoryStream ms = new MemoryStream();
                file.CopyTo(ms);
                advertisement.photo = ms.ToArray();
                ms.Close();
                ms.Dispose();
            }
            return await _advertisementRepository.Add(advertisement);
        }


        [HttpPut]
        public Task<Advertisement> Put([FromForm] Advertisement advertisement)
        {
            return _advertisementRepository.Update(advertisement);
        }

        [HttpDelete("{id}")]
        public Task<Advertisement> Delete(int id)
        {
            return _advertisementRepository.Delete(id);
        }
    }
}
