using application.DTOs;
using application.DTOs.Chat;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alamuti.Application.Interfaces.repository
{
    public interface IChatRepository
    {
        Task<ChatGroup> AddGroup(ChatGroup group);
        Task ToChecked(string groupname);
        Task<bool> ToUnchecked(string groupname);
        IEnumerable<ChatGroup> GetGroupsNoPagination(string userId);
        Task<PaginatedList<ChatGroup>> GetGroups(string userId, MessageParameters messageParameters);
        Task<bool> DeleteGroup(string groupName);
        Task<bool> ReportChat(string groupName, string blockedUserId, string report);
        Task<PaginatedList<ChatMessage>> GetMessages(string groupName, MessageParameters messageParameters);
        Task AddMessage(string groupName, ChatMessage message);
        Task<ChatGroup> GetGroupByName(string groupname);
        Task AddAlamutiChat(Advertisement ads, PublishStatus publishStatus);
    }
}
