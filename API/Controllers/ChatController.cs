using application.DTOs;
using application.DTOs.Chat;
using application.Interfaces.Data;
using application.Interfaces.repository;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatController : ControllerBase
    {
        private readonly MessageRepository _messageRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public ChatController(MessageRepository messageRepository, UserManager<IdentityUser> userManager)
        {
            _messageRepository = messageRepository;
            _userManager = userManager;

        }

        [HttpGet("massages")]
        public async Task<IEnumerable<ChatMessage>> Get()
        {
            return await _messageRepository.GetAllMessages();
        }


        [HttpGet("massages/{groupname}")]
        public async Task<IEnumerable<ChatMessageDto>> Get(string groupname)
        {
            var messages = await _messageRepository.GetMessages(groupname);
            return messages.Select(x=>new ChatMessageDto() 
            { 
                Sender = x.Sender,
                Reciever = x.Reciever,
                Id = x.Id,
                DateSended = x.DateSended,
                GroupName = groupname,
                Message = x.Message,
                DaySended = x.DateSended.ToString()
            });

        }

        [HttpDelete("group/{groupname}")]
        public async Task<IActionResult> DeleteGroup(string groupname)
        {
            var deletedGroup = Ok(await _messageRepository.DeleteGroup(groupname));

            if (deletedGroup != null)
            {
                return Ok(deletedGroup);
            }
            return NotFound();
        }



        [HttpGet("groups")]
        public async Task<IActionResult> GetGroups()
        {
            var userId = User.Claims.FirstOrDefault()?.Value;
            return Ok(await _messageRepository.GetAllGroup(userId));
           
        }

        [HttpGet("groupswithmessages")]
        public async Task<IActionResult> GetGroupsWithMessages()
        {
            var userId = User.Claims.FirstOrDefault()?.Value;
            var groups = await _messageRepository.GetGroupWithMessages(userId);
           
            var result = groups.Select(group=>new ChatGroupDto()
                {Name = group.Name,
                Id = group.Id,
                IsChecked = group.IsChecked,
                Image = group.Image,
                Title = group.Title,
                
                    LastMessage = new ChatMessageDto()
                    {
                        DateSended = group.Messages.Last().DateSended,
                        DaySended = group.Messages.Last().DateSended.ToString(),
                        GroupName = group.Messages.Last().GroupName,
                        Message = group.Messages.Last().Message,
                        Id = group.Messages.Last().Id,
                        Reciever = group.Messages.Last().Reciever,
                        Sender = group.Messages.Last().Sender,
                    }
            });
           
            return Ok(result);
        }


        [HttpPost("addgroup")]
        public async Task<IActionResult> Post([FromForm] ChatGroup group)
        {
            return Ok(await _messageRepository.AddGroup(group));
            
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromForm] ChatGroup group)
        {
            return Ok(await _messageRepository.UpdateGroup(group)); 
        }


        [HttpGet("groups/{groupname}")]
        public async Task<IActionResult> GetLastMessage( string groupname )
        {
            return Ok(await _messageRepository.GetLastMessageOfGroup(groupname));

        }


        [HttpPost()]
        public async Task<ChatMessage> Post([FromBody] ChatMessage message)
        {
            Ok(await _messageRepository.Add(message));
            return message;
        }
    }
}
