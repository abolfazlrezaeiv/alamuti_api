using application.DTOs;
using application.DTOs.Advertisement;
using application.Interfaces.repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class AdvertisementRepository : IAdvertisementRepository
    {
        private readonly AlamutDbContext _context;
        private readonly MessageRepository _messageRepository;

        public AdvertisementRepository(AlamutDbContext context, MessageRepository messageRepository)
        {
            _context = context;
            _messageRepository = messageRepository;
        }


        public async Task<Advertisement> Add(Advertisement entity)
        {

            await _context.Advertisements.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }


        public async Task<Advertisement> Delete(Advertisement entity)
        {

            var entityInDatabase = await _context.Advertisements.FindAsync(entity.Id);
            if (entityInDatabase == null)
            {
                return entity;
            }

            _context.Advertisements.Remove(entity);
            await _context.SaveChangesAsync();

            return entity;
        }


        public async Task<Advertisement> Get(int id)
        {
            return await _context.Advertisements.FindAsync(id);
        }


        public async Task<Advertisement> ChangeToPublished(int id)
        {
            var advertisement = await _context.Advertisements.FindAsync(id);

            if (advertisement == null)
                return null;


            var alamutiGroup =  _context.ChatGroups.Where(x => x.Name == "alamuti" + advertisement.UserId);

            if (await alamutiGroup.CountAsync() == 0)
            {
                await _messageRepository.AddGroup(new ChatGroup() { Name = "alamuti" + advertisement.UserId, Title = "الموتی", IsChecked = false });
            }

            var alamutiExistedGroup = await _context.ChatGroups.Where(x => x.Name == "alamuti" + advertisement.UserId).FirstAsync();

            await _messageRepository.AddMessageToGroup("alamuti" + advertisement.UserId,
                new ChatMessage() 
                { 
                    ChatGroup = alamutiExistedGroup,
                    ChatGroupId = alamutiExistedGroup.Id,
                    Sender = "الموتی",
                    GroupName = alamutiExistedGroup.Name,
                    Reciever = advertisement.UserId,
                    Message = $"آگهی {advertisement.Title} منتشر شد" 
                });

            advertisement.Published = true;

            await _context.SaveChangesAsync();

            return advertisement;
        }


        public async Task<Advertisement> DeleteUnpublished(int id)
        {

            var advertisement = await _context.Advertisements.FindAsync(id);
            if (advertisement == null)
            {
                return null;
            }

            var alamutiGroup = _context.ChatGroups.Where(x => x.Name == "alamuti" + advertisement.UserId);
            if (await alamutiGroup.CountAsync() == 0)
            {
                await _messageRepository.AddGroup(new ChatGroup() { Name = "alamuti" + advertisement.UserId, Title = "الموتی", IsChecked = false });
            }


            var alamutiExistedGroup = await _context.ChatGroups.Where(x => x.Name == "alamuti" + advertisement.UserId).FirstAsync();

        

            await _messageRepository.AddMessageToGroup("alamuti" + advertisement.UserId, new ChatMessage() { ChatGroup = alamutiExistedGroup, ChatGroupId = alamutiExistedGroup.Id, Sender = "الموتی", GroupName = alamutiExistedGroup.Name, Reciever = advertisement.UserId, Message = $"آگهی {advertisement.Title} به یکی از دلایل زیر تایید نشد :\n- مغایرت با قوانین الموتی\n- آگهی تکراری است" });
            _context.Advertisements.Remove(advertisement);
            await _context.SaveChangesAsync();

            return advertisement;
        }

        public async Task<PaginatedList<Advertisement>> GetCurrentUserAds(IdentityUser user, AdvertisementParameters advertisementParameters)
        {
            return await PaginatedList<Advertisement>
            .CreateAsync(_context.Advertisements.AsNoTracking().Where(x => x.UserId == user.Id),
            advertisementParameters.PageNumber,
            advertisementParameters.PageSize);
        }

        public  async Task<PaginatedList<Advertisement>> GetUnpublishedUserAds(string userId, AdvertisementParameters advertisementParameters)
        {
            var a = userId;
            return await PaginatedList<Advertisement>
            .CreateAsync(_context.Advertisements.AsNoTracking().Where(x => userId == x.UserId && x.Published == true),
            advertisementParameters.PageNumber,
            advertisementParameters.PageSize);
        }

        public  async Task<PaginatedList<Advertisement>> Search(string input, AdvertisementParameters advertisementParameters)
        {
            var searchResult = _context.Advertisements.AsNoTracking()
                .Where(x => (x.Title.Contains(input) || x.Village.Contains(input)) && x.Published == true);

            return await PaginatedList<Advertisement>
              .CreateAsync(searchResult,
              advertisementParameters.PageNumber,
              advertisementParameters.PageSize);

        }


        public async Task<PaginatedList<Advertisement>> GetAll(string adsType, AdvertisementParameters advertisementParameters)
        {
            return await PaginatedList<Advertisement>
                .CreateAsync(_context.Advertisements.AsNoTracking().Where(x => x.AdsType == adsType && x.Published == true)
                .OrderByDescending(x => x.DatePosted),
                advertisementParameters.PageNumber,
                advertisementParameters.PageSize);
        }


        public async Task<PaginatedList<Advertisement>> GetAll(AdvertisementParameters advertisementParameters)
        {

            return await PaginatedList<Advertisement>
               .CreateAsync(_context.Advertisements.AsNoTracking().Where(x => x.Published == true)
               .OrderByDescending(x => x.DatePosted),
               advertisementParameters.PageNumber,
               advertisementParameters.PageSize);
        }

        public async Task<PaginatedList<Advertisement>> GetAllUnpublished(AdvertisementParameters advertisementParameters)
        {
            return await PaginatedList<Advertisement>
               .CreateAsync(_context.Advertisements.AsNoTracking()
                   .Where(x => x.Published == false)
                   .OrderBy(x => x.DatePosted)
                   ,
               advertisementParameters.PageNumber,
               advertisementParameters.PageSize);
        }



        public async Task<Advertisement> Update(Advertisement advertisement)
        {
            var result = await _context.Advertisements.FindAsync(advertisement.Id);
            if (result != null)
            {
                result.Id = advertisement.Id;
                result.Title = advertisement.Title;
                result.Area = advertisement.Area;
                result.Price = advertisement.Price;
                result.Description = advertisement.Description;
                result.Photo1 = advertisement.Photo1;
                result.Photo2 = advertisement.Photo2;
                result.ListViewPhoto = advertisement.ListViewPhoto;
                result.Area = advertisement.Area;
                result.Village = advertisement.Village;
                result.Published = false;
                await _context.SaveChangesAsync();
            }
            return advertisement;
        }

        public async Task<Advertisement> ReportAdvertisement(int id , string report)
        {
            var result = await _context.Advertisements.FindAsync(id);
            if (result != null)
            {
                result.ReportMessage = report;
                result.Published = true;
                await _context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<Advertisement> RemoveReportAdvertisement(int id)
        {
            var result = await _context.Advertisements.FindAsync(id);
            if (result != null)
            {
                result.ReportMessage = null;
                await _context.SaveChangesAsync();
            }

            return result;
        }


        public async Task<PaginatedList<Advertisement>> GetReportedAdvertisements(AdvertisementParameters advertisementParameters)
        {
            return await PaginatedList<Advertisement>
                 .CreateAsync(_context.Advertisements.AsNoTracking()
                     .Where(x => x.ReportMessage != null)
                     .OrderBy(x => x.DatePosted),
                 advertisementParameters.PageNumber,
                 advertisementParameters.PageSize);
        }

        public async Task<IEnumerable<Advertisement>> GetAll()
        {
            return await _context.Advertisements.Where(x => x.Published == true)
                .OrderByDescending(x => x.DatePosted)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
