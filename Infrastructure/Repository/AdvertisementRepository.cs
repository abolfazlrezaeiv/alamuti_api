using application.DTOs.Requests;
using application.Interfaces.Data;
using application.Interfaces.repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class AdvertisementRepository : IAdvertisementRepository
    {
        private readonly AlamutDbContext _context;
        private readonly MessageRepository _messageRepository;

        public AdvertisementRepository(AlamutDbContext context,MessageRepository messageRepository)
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
            var advertisement = await  _context.Advertisements.FindAsync(id);
            if (advertisement == null)
                return null;


            if (await _messageRepository.UpdateGroupIsChecked("alamuti" + advertisement.UserId) != true)
            {
                await _messageRepository.AddGroup(new ChatGroup() { Name = "alamuti" + advertisement.UserId, Title = "الموتی", IsChecked = false });
            }


            var group = await _context.ChatGroups.Where(x => x.Name == "alamuti" + advertisement.UserId).FirstAsync();

            await _context.Messages.AddAsync(new ChatMessage() { ChatGroup = group, ChatGroupId = group.Id, Sender = "الموتی", GroupName = group.Name, Reciever = advertisement.UserId, Message = $"آگهی {advertisement.Title} منتشر شد" });


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

            _context.Advertisements.Remove(advertisement);
            if (await _messageRepository.UpdateGroupIsChecked("alamuti" + advertisement.UserId) != true)
            {
                await _messageRepository.AddGroup(new ChatGroup() { Name = "alamuti" + advertisement.UserId, Title = "الموتی", IsChecked = false });
                await _context.SaveChangesAsync();
            }
            

            var group = await _context.ChatGroups.Where(x => x.Name == "alamuti" + advertisement.UserId).FirstAsync();

            await _context.Messages.AddAsync(new ChatMessage() {ChatGroup = group , ChatGroupId = group.Id , Sender = "الموتی",GroupName = group.Name,Reciever = advertisement.UserId,Message = $"آگهی {advertisement.Title} به یکی از دلایل زیر تایید نشد :\n- مغایرت با قوانین الموتی\n- آگهی تکراری است" });
            await _context.SaveChangesAsync();

            return advertisement;
        }

        public async Task<IEnumerable<Advertisement>> GetCurrentUserAds(IdentityUser user)
        {
            return await _context.Advertisements.Where(x=>x.UserId == user.Id).ToListAsync();
        }

        public async Task<IEnumerable<Advertisement>> GetUnpublishedUserAds(string userId)
        {
            return await _context.Advertisements.Where(x => x.UserId == userId && x.Published == true).ToListAsync();
        }

        public async Task<IEnumerable<Advertisement>> Search(string input)
        {
            var searchResult =  await _context.Advertisements
                .Where(x => x.Title.Contains(input)&& x.Published == true)
                .ToListAsync();

            return searchResult;
        }


        public async Task<IEnumerable<Advertisement>> GetAll(string adsType)
        {

            return await _context.Advertisements.Where(x=>x.AdsType == adsType && x.Published == true).ToListAsync();
        }


        public async Task<IEnumerable<Advertisement>> GetAll()
        {

            return await _context.Advertisements.Where(x=>x.Published == true).ToListAsync();
        }

        public async Task<IEnumerable<Advertisement>> GetAllUnpublished()
        {

            return await _context.Advertisements.Where(x => x.Published == false).ToListAsync();
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
                result.photo1 = advertisement.photo1;
                result.photo2 = advertisement.photo2;
                result.Published = false;
                await _context.SaveChangesAsync();
            }
         
            return advertisement;
        }
    }
}
