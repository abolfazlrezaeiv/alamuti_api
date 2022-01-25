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
            var existedChatGroup =  await  _context.ChatGroups.Where(x=> x.Name == group.Name).ToListAsync();
            if (existedChatGroup.Count == 0)
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

        public async Task<IEnumerable<ChatMessage>> GetAllMessages()
        {
            return await _context.Messages.ToListAsync();
        }


        public PagedList<ChatMessage> GetMessages(string groupName, MessageParameters messageParameters)
        {
            return PagedList<ChatMessage>
                .ToPagedList(
                _context.Messages.Where(x => x.GroupName == groupName)
                .AsNoTracking().OrderByDescending(x => x.DateSended),
               messageParameters.PageNumber,
               messageParameters.PageSize);
        }


        public async Task<ChatMessage> GetLastMessageOfGroup (string groupName)
        {
            return await _context.Messages.Where(x => x.GroupName == groupName).OrderBy(x=>x.DateSended).LastAsync();
        }


        public async Task DeleteGroup(string groupName)
        {

            var group = await _context.ChatGroups.Include(x => x.Messages).Where(x => x.Name == groupName).FirstAsync();
            if (group != null)
            {
                _context.ChatGroups.Remove(group);
                var messages = _context.Messages.Where(x => x.GroupName == group.Name);
                _context.Messages.RemoveRange(messages);
                await _context.SaveChangesAsync();
            }
        }

       
        public PagedList<ChatGroup> GetGroupWithMessages(string userId, MessageParameters messageParameters)
        {
            return PagedList<ChatGroup>
              .ToPagedList(
              _context.ChatGroups
                .Include(b => b.Messages.OrderBy(x => x.DateSended))
                .Where(x => x.Name.Contains(userId) && x.Messages.Count != 0)
                .OrderByDescending(x => x.Messages.OrderBy(x => x.DateSended).Last().DateSended)
                .AsNoTracking(),
             messageParameters.PageNumber,
             messageParameters.PageSize);

        }


        public async Task<List<ChatGroup>> GetGroups(string userId)
        {
            return await _context.ChatGroups
                .Include(b => b.Messages.OrderBy(x => x.DateSended))
                .Where(x => x.Name.Contains(userId) && x.Messages.Count != 0)
                .OrderByDescending(x => x.Messages.OrderBy(x => x.DateSended).Last().DateSended)
                .AsNoTracking().ToListAsync();

        }



        public async Task<bool> UpdateGroupIsChecked(string groupname)
        {
            var group = await _context.ChatGroups.Where(x => x.Name == groupname).ToListAsync();

           
            if (group.Count != 0)
            {
                group.First().IsChecked = false;

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task UpdateGroup(ChatGroup group)
        {
             var groupToChange =  await _context.ChatGroups.Where(x => x.Name == group.Name).FirstAsync();

            if (groupToChange != null)
            {
                groupToChange.IsChecked = group.IsChecked;
               
                await _context.SaveChangesAsync();
            }
        }
    }
}
