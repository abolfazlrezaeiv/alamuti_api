using application.Interfaces.repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class MessageRepository 
    {
        private readonly AlamutDbContext _context;

        public MessageRepository(AlamutDbContext context)
        {
            _context = context;
        }

        public async Task<ChatGroup> AddGroup(ChatGroup group)
        {
            var availableGroup =  await  _context.ChatGroups.Where(x=>x.Name == group.Name).ToListAsync();
            if (availableGroup.Count == 0)
            {
                await _context.ChatGroups.AddAsync(group);
                await _context.SaveChangesAsync();
                return group;
            }
            return group;
           
        }

        public async Task<ChatMessage> Add(ChatMessage entity)
        {
            await _context.Messages.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ChatMessage> AddMessageToGroup(string groupName,ChatMessage message)
        {
            var availableGroup = await  _context.ChatGroups.Where(x => x.Name == groupName).FirstAsync();
            await _context.Messages.AddAsync(new ChatMessage() 
            { 
                Sender = message.Sender,
                Message = message.Message,
                ChatGroup = availableGroup,
                ChatGroupId= availableGroup.Id,
                Reciever = message.Reciever,
                GroupName = groupName,
                DateSended = message.DateSended
            });
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<IEnumerable<ChatMessage>> GetAllMessages()
        {

            return await _context.Messages.ToListAsync();
        }


        public async Task<IEnumerable<ChatMessage>> GetMessages(string groupName)
        {

             return await _context.Messages.Where(x=>x.GroupName == groupName).ToListAsync();
        }

        public async Task<ChatMessage> GetLastMessageOfGroup (string groupName)
        {

            return await _context.Messages.Where(x => x.GroupName == groupName).OrderBy(x=>x.DateSended).LastAsync();
        }

        public async Task<ChatGroup> DeleteGroup(string groupName)
        {

            var group = await _context.ChatGroups.Where(x => x.Name == groupName).FirstAsync();
            if (group != null)
            {
                _context.ChatGroups.Remove(group);
                await _context.SaveChangesAsync();

                return group;
            }
            return null;
        }

        public  async Task<IEnumerable<ChatGroup>> GetAllGroup(string userId)
        {
            return await _context.ChatGroups.Where(x => x.Name.Contains(userId)).ToListAsync();

        }
        public async Task<List<ChatGroup>> GetGroupWithMessages(string userId)
        {
            var groups =await _context.ChatGroups.Where(x => x.Name.Contains(userId))
                     .Include(b => b.Messages.OrderBy(x=>x.DateSended))
                     .OrderByDescending(x=>x.Messages.OrderBy(x=>x.DateSended).Last().DateSended)
                     .ToListAsync();
                     
            return groups;
        }



        public async Task<bool> UpdateGroupIsChecked(string groupname)
        {
            var group = await _context.ChatGroups.Where(x => x.Name == groupname).FirstAsync();

           
            if (group != null)
            {
                group.IsChecked = false;

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<ChatGroup> UpdateGroup(ChatGroup group)
        {
             var groupToChange =  await _context.ChatGroups.Where(x => x.Name == group.Name).FirstAsync();
             
            
            if (groupToChange != null)
            {
                groupToChange.IsChecked = group.IsChecked;
               
                await _context.SaveChangesAsync();
            }
            return group;
        }

    }
}
