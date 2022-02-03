using application.DTOs;
using application.DTOs.Chat;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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

        public async Task AddGroup(ChatGroup group)
        {
            var existedChatGroup =  _context.ChatGroups.AsNoTracking().Where(x=> x.Name == group.Name);
            if (await existedChatGroup.CountAsync()  == 0)
            {
                await _context.ChatGroups.AddAsync(group);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Add(ChatMessage entity)
        {
            await _context.Messages.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<ChatMessage> AddMessageToGroup(string groupName,ChatMessage message)
        {
            var availableGroup = await  _context.ChatGroups.Where(x => x.Name == groupName ).FirstAsync();
         
            availableGroup.IsChecked = false;
                
            await _context.SaveChangesAsync();

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

        public IEnumerable<ChatMessage> GetAllMessages()
        {
            return _context.Messages.AsNoTracking();
        }


        public  async Task<PaginatedList<ChatMessage>> GetMessages(string groupName, MessageParameters messageParameters)
        {
            return await PaginatedList<ChatMessage>
                .CreateAsync(
                _context.Messages.AsNoTracking().Where(x => x.GroupName == groupName)
                .OrderByDescending(x => x.DateSended),
               messageParameters.PageNumber,
               messageParameters.PageSize);
        }


        public async Task<ChatMessage> GetLastMessageOfGroup (string groupName)
        {
            return await _context.Messages.AsNoTracking().Where(x => x.GroupName == groupName).OrderBy(x=>x.DateSended).LastAsync();
        }


        public async Task DeleteGroup(string groupName)
        {

            var group = await _context.ChatGroups.Include(x => x.Messages).AsNoTracking().Where(x => x.Name == groupName).FirstAsync();
            if (group != null)
            {
                _context.ChatGroups.Remove(group);
                var messages = _context.Messages.Where(x => x.GroupName == group.Name).AsNoTracking();
                _context.Messages.RemoveRange(messages);
                await _context.SaveChangesAsync();
            }
        }

       
        public async Task<PaginatedList<ChatGroup>> GetGroupWithMessages(string userId, MessageParameters messageParameters)
        {
            return await PaginatedList<ChatGroup>
              .CreateAsync(
              _context.ChatGroups.AsNoTracking()
                .Include(b => b.Messages.AsQueryable().OrderBy(x => x.DateSended).AsNoTracking())
                .Where(x => x.Name.Contains(userId) && x.Messages.Count != 0)
                .OrderByDescending(x => x.Messages.OrderBy(x => x.DateSended).Last().DateSended)
                ,
             messageParameters.PageNumber,
             messageParameters.PageSize);

        }


        public IEnumerable<ChatGroup> GetGroups(string userId)
        {
            return  _context.ChatGroups.AsNoTracking()
                .Include(b => b.Messages.OrderBy(x => x.DateSended))
                .Where(x => x.Name.Contains(userId) && x.Messages.Count != 0)
                .OrderByDescending(x => x.Messages.OrderBy(x => x.DateSended).Last().DateSended);


        }



        public async Task<bool> UpdateGroupIsChecked(string groupname)
        {
            var group =  _context.ChatGroups.Where(x => x.Name == groupname);

           
            if (await group.CountAsync() != 0)
            {
                group.First().IsChecked = false;

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task UpdateGroup(string groupname)
        {
             var groupToChange =  await   _context.ChatGroups.Where(x => x.Name == groupname).FirstAsync();

            if (groupToChange != null)
            {
                groupToChange.IsChecked = true;
               
                await _context.SaveChangesAsync();
            }
        }
    }
}
