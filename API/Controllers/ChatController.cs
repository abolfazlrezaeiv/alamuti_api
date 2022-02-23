using application.DTOs.Chat;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/chat")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatController : BaseController
    {
        private readonly MessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public ChatController(MessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

       

        [HttpGet("groups/{groupname}")]
        public async Task<ChatGroup> GetGroupByName(string groupname)
        {
            return await _messageRepository.GetGroupByName(groupname);
        }

        [HttpGet("groups/{groupname}/messages")]
        public async Task<IEnumerable<ChatMessageDto>> GetMessages(string groupname, [FromQuery] MessageParameters messageParameters)
        {
            var messages = await _messageRepository.GetMessages(groupname, messageParameters);

            AddHeaderPagination(messages);

            return messages.Select(x => _mapper.Map<ChatMessageDto>(x));
        }

        [HttpGet("groups")]
        public async Task<IEnumerable<ChatGroupDto>> GetGroups([FromQuery] MessageParameters messageParameters)
        {
            var userId = User.Claims.FirstOrDefault()?.Value;
            var groups = await _messageRepository.GetGroups(userId, messageParameters);

            AddHeaderPagination(groups);

            return groups.Select(group => _mapper.Map<ChatGroupDto>(group));
        }



        [HttpGet("groups/no-paginated")]
        public  IEnumerable<ChatGroupDto> GetAllGroups()
        {
            var userId = User.Claims.FirstOrDefault()?.Value;
            var groups =  _messageRepository.GetGroupsNoPagination(userId);
            return groups.Select(group => _mapper.Map<ChatGroupDto>(group));
        }


        [HttpDelete("groups/{groupname}")]
        public async Task Delete(string groupname)
        {
            await _messageRepository.DeleteGroup(groupname);
        }


        [HttpPost("groups")]
        public async void Insert([FromForm] ChatGroup group)
        {
            await _messageRepository.AddGroup(group);
        }

        [HttpPut("groups/{groupname}")]
        public async Task Update(string groupname)
        {
            await _messageRepository.ConvertGroupToChecked(groupname);
        }

        [HttpPut("groups/report")]
        public async Task ReportChat([FromForm] string groupname, [FromForm]  string blockedUserId, [FromForm] string reportMessage)
        {
            await _messageRepository.ReportChat(groupname, blockedUserId, reportMessage);
        }
    }
}
