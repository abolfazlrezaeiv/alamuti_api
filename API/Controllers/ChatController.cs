using Alamuti.Application.Interfaces.UnitOfWork;
using application.DTOs.Chat;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/chat")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChatController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("groups/{groupname}")]
        public async Task<IActionResult> GetGroupByName(string groupname)
        {
            var result = await _unitOfWork.Chat.GetGroupByName(groupname);
            if (result == null) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("groups/{groupname}/messages")]
        public async Task<IEnumerable<ChatMessageDto>> GetMessages(string groupname, [FromQuery] MessageParameters messageParameters)
        {
            var messages = await _unitOfWork.Chat.GetMessages(groupname, messageParameters);
            AddHeaderPagination(messages);
            return messages.Select(message => _mapper.Map<ChatMessageDto>(message));
        }

        [HttpGet("groups")]
        public async Task<IEnumerable<ChatGroupDto>> GetGroups([FromQuery] MessageParameters messageParameters)
        {
            var userId = User.Claims.FirstOrDefault()?.Value;
            var groups = await _unitOfWork.Chat.GetGroups(userId, messageParameters);
            AddHeaderPagination(groups);
            return groups.Select(group => _mapper.Map<ChatGroupDto>(group));
        }

        [HttpGet("groups/no-paginated")]
        public  IEnumerable<ChatGroupDto> GetAllGroups()
        {
            var userId = User.Claims.FirstOrDefault()?.Value;
            var groups = _unitOfWork.Chat.GetGroupsNoPagination(userId);
            return groups.Select(group => _mapper.Map<ChatGroupDto>(group));
        }

        [HttpDelete("groups/{groupname}")]
        public async Task<IActionResult> Delete(string groupname)
        {
            var result = await _unitOfWork.Chat.DeleteGroup(groupname);
            if(result ==false) return NotFound();
            await _unitOfWork.CompleteAsync();
            return Ok();
        }


        [HttpPost("groups")]
        public async void Insert([FromForm] ChatGroup group)
        {
            await _unitOfWork.Chat.AddGroup(group);
            await _unitOfWork.CompleteAsync();
        }

        [HttpPut("groups/{groupname}")]
        public async Task Update(string groupname)
        {
            await _unitOfWork.Chat.ToChecked(groupname);
            await _unitOfWork.CompleteAsync();
        }

        [HttpPut("groups/report")]
        public async Task<IActionResult> ReportChat([FromForm] string groupname, [FromForm]  string blockedUserId, [FromForm] string reportMessage)
        {
            var result = await _unitOfWork.Chat.ReportChat(groupname, blockedUserId, reportMessage);
            if(result==false) return NotFound();
            await _unitOfWork.CompleteAsync();
            return Ok();
        }
    }
}
