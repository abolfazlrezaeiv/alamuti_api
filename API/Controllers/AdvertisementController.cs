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
        public Task<IEnumerable<Advertisement>> Get() 
        { 
            return _advertisementRepository.GetAll();
        }


        [HttpGet("{id}")]
        public async Task<Advertisement> Get(int id)
        {
            return await _advertisementRepository.Get(id) ;
        }


        [HttpPost]
        public  async Task<Advertisement> Post([FromForm] Advertisement  advertisement)
        {
           
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(userId);
            advertisement.UserId = user.Id;

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

        //[HttpPost("imagesend")]
        //public async Task<Advertisement> AddImage([FromForm] Advertisement advertisement)
        //{


        //    foreach (var file in Request.Form.Files)
        //    {
        //        Image img = new Image();
        //        img.ImageTitle = file.FileName;
        //        MemoryStream ms = new MemoryStream();
        //        file.CopyTo(ms);
        //        advertisement.photo = ms.ToArray();
        //        ms.Close();
        //        ms.Dispose();
        //    }
        //    return await _advertisementRepository.Update(advertisement);
        //}

        [HttpPut]
        public Task<Advertisement> Put([FromBody] Advertisement advertisement)
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
