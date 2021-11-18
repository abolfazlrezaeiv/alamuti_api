using application.Interfaces.Data;
using application.Interfaces.repository;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementController : ControllerBase
    {
        private readonly IRepository<Advertisement> _advertisementRepository;

        public AdvertisementController(IRepository<Advertisement> advertisementRepository)
        {
            _advertisementRepository = advertisementRepository;
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
        public Task<Advertisement> Post([FromBody] Advertisement  advertisement)
        {
            return _advertisementRepository.Add(advertisement);
        }


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
