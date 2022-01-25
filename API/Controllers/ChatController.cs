﻿using application.DTOs.Chat;
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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatController : ControllerBase
    {
        private readonly MessageRepository _messageRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;

        public ChatController(MessageRepository messageRepository, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet("massages")]
        public async Task<IEnumerable<ChatMessage>> Get()
        {
            return await _messageRepository.GetAllMessages();
        }


        [HttpGet("massages/{groupname}")]
        public IEnumerable<ChatMessageDto> Get(string groupname, [FromQuery] MessageParameters messageParameters)
        {
            var messages = _messageRepository.GetMessages(groupname, messageParameters);

            var metadata = new
            {
                messages.TotalCount,
                messages.PageSize,
                messages.CurrentPage,
                messages.TotalPages,
                messages.HasNext,
                messages.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return messages.Select(x => _mapper.Map<ChatMessageDto>(x));
        }

        [HttpGet("groupswithmessages")]
        public IEnumerable<ChatGroupDto> GetGroupsWithMessages([FromQuery] MessageParameters messageParameters)
        {
            var userId = User.Claims.FirstOrDefault()?.Value;
            var groups = _messageRepository.GetGroupWithMessages(userId, messageParameters);

            var metadata = new
            {
                groups.TotalCount,
                groups.PageSize,
                groups.CurrentPage,
                groups.TotalPages,
                groups.HasNext,
                groups.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return groups.Select(group => _mapper.Map<ChatGroupDto>(group));
        }



        [HttpGet("groups")]
        public async Task<IEnumerable<ChatGroupDto>> GetUserGroups()
        {
            var userId = User.Claims.FirstOrDefault()?.Value;
            var groups = await _messageRepository.GetGroups(userId);
            return groups.Select(group => _mapper.Map<ChatGroupDto>(group));
        }


        [HttpDelete("group/{groupname}")]
        public async Task<IActionResult> DeleteGroup(string groupname)
        {
            await _messageRepository.DeleteGroup(groupname);
            return Ok();

        }



        [HttpPost("addgroup")]
        public async Task<IActionResult> Post([FromForm] ChatGroup group)
        {
            await _messageRepository.AddGroup(group);
            return Ok();
        }

        [HttpPut("group/{groupname}")]
        public async Task<IActionResult> Put([FromBody] string groupname)
        {
            await _messageRepository.UpdateGroup(groupname);
            return Ok();
        }


        [HttpGet("groups/{groupname}")]
        public async Task<IActionResult> GetLastMessage(string groupname)
        {
            return Ok(await _messageRepository.GetLastMessageOfGroup(groupname));

        }


        [HttpPost()]
        public async Task<IActionResult> Post([FromBody] ChatMessage message)
        {
            await _messageRepository.Add(message);
            return Ok();
        }
    }
}
