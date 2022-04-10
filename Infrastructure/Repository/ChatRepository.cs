using Alamuti.Application.Interfaces.repository;
using Alamuti.Infrastructure.Repository;
using application.DTOs;
using application.DTOs.Chat;
using application.Interfaces.repository;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repository;
    public class ChatRepository : GenericRepository<ChatGroup>, IChatRepository
    {
        private readonly AlamutDbContext _context;

        public ChatRepository(AlamutDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ChatGroup> AddGroup(ChatGroup group)
        {
            var existedChatGroup = await _context.ChatGroups.AsNoTracking().FirstOrDefaultAsync(x => x.Name == group.Name);
            if (existedChatGroup is not null) return group;
            await _context.ChatGroups.AddAsync(group);
            return group;
        }

        public async Task<ChatGroup> GetGroupByName(string groupname)
        {
                return await _context.ChatGroups.AsNoTracking().FirstOrDefaultAsync(x => x.Name == groupname);
        }

        public async Task AddMessage(string groupName, ChatMessage message)
        {
            var availableGroup = await _context.ChatGroups.Where(x => x.Name == groupName).FirstAsync();
            availableGroup.IsChecked = false;
            await _context.Messages.AddAsync(new ChatMessage()
            {
                Sender = message.Sender,
                Message = message.Message,
                ChatGroup = availableGroup,
                ChatGroupId = availableGroup.Id,
                Reciever = message.Reciever,
                GroupName = groupName,
                DateSended = message.DateSended
            });
        }


        public async Task<PaginatedList<ChatMessage>> GetMessages(string groupName, MessageParameters messageParameters)
                => await PaginatedList<ChatMessage>.CreateAsync(_context.Messages.AsNoTracking().Where(x => x.GroupName == groupName)
                .OrderByDescending(x => x.DateSended),
               messageParameters.PageNumber,
               messageParameters.PageSize);

        public async Task<bool> ReportChat(string groupName, string blockedUserId, string report)
        {
            var result =await  _context.ChatGroups.FirstOrDefaultAsync(x => x.Name == groupName);
            if (result is null) return false;
            result.ReportMessage = report;
            result.IsDeleted = true;
            result.BlockedUserId = blockedUserId;
            return true;
        }

        public async Task<bool> DeleteGroup(string groupName)
        {
            var group = await _context.ChatGroups.FirstOrDefaultAsync(x => x.Name == groupName);
            if(group is null) return false;
            group.IsDeleted = true;
            return true;
        }

        public async Task<PaginatedList<ChatGroup>> GetGroups(string userId, MessageParameters messageParameters)
               => await PaginatedList<ChatGroup>
              .CreateAsync(_context.ChatGroups.AsNoTracking().Include(b => b.Messages.AsQueryable().OrderBy(x => x.DateSended).AsNoTracking())
                .Where(x => x.Name.Contains(userId) && x.Messages.Count != 0 && x.IsDeleted == false)
                .OrderByDescending(x => x.Messages.OrderBy(x => x.DateSended).Last().DateSended),
             messageParameters.PageNumber,
             messageParameters.PageSize);

        public IEnumerable<ChatGroup> GetGroupsNoPagination(string userId)
                => _context.ChatGroups.AsNoTracking().Include(b => b.Messages.OrderBy(x => x.DateSended))
                .Where(x => x.Name.Contains(userId) && x.Messages.Count > 0 && x.IsDeleted == false)
                .OrderByDescending(x => x.Messages.OrderBy(x => x.DateSended).Last().DateSended);

        public async Task<bool> ToUnchecked(string groupname)
        {
            var group = await _context.ChatGroups.FirstOrDefaultAsync(x => x.Name == groupname);
            if (group is null) return false;
            group.IsChecked = false;
            return true;
        }

        public async Task ToChecked(string groupname)
        {
            var groupToChange = await _context.ChatGroups.Where(x => x.Name == groupname).FirstOrDefaultAsync();
            if (groupToChange is null) return;
            groupToChange.IsChecked = true;
        }

        public async Task AddAlamutiChat(Advertisement ads, PublishStatus publishStatus)
        {
            var groupName = "alamuti" + ads.UserId;
            var group = await AddGroup(new ChatGroup() { Name = groupName, Title = alamuti, IsChecked = false });
            await AddMessage(groupName, new ChatMessage()
            {
                ChatGroup = group,
                Sender = alamuti,
                GroupName = group.Name,
                Reciever = ads.UserId,
                Message = publishStatus == PublishStatus.accept ? GetAcceptMessage(ads.Title) : GetRejectMessage(ads.Title), 
            });
        }

        const string alamuti = "الموتی";
        static string GetRejectMessage(string title) 
            => $"آگهی {title} به یکی از دلایل زیر تایید نشد :\n- مغایرت با قوانین الموتی\n- آگهی تکراری است";
        
        static string GetAcceptMessage(string title) => $"آگهی {title} منتشر شد";
    }

