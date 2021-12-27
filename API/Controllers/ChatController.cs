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
            return  _messageRepository.GetMessages(groupname).Result.Select(x=>new ChatMessageDto() 
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


        [HttpGet("groups")]
        public async Task<IEnumerable<ChatGroup>> GetGroups()
        {
            var userId = User.Claims.FirstOrDefault().Value;
            var currentuser = await _userManager.FindByIdAsync(userId);
            return  _messageRepository.GetAllGroup().Result.Where(x=>x.Name.Contains(currentuser.Id)) ;

        }

        [HttpGet("groupswithmessages")]
        public async Task<IEnumerable<ChatGroupDto>> GetGroupsWithMessages()
        {
            var userId = User.Claims.FirstOrDefault().Value;
            var currentuser = await _userManager.FindByIdAsync(userId);
            return _messageRepository.GetGroupWithMessages().Result.Where(x => x.Name.Contains(currentuser.Id))
                .Select(group=>new ChatGroupDto()
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
        }



        //[HttpPost("/addMessageToGroup")]
        //public async Task<ChatMessage> Post([FromBody] ChatMessage chatMessage)
        //{


        //   await _messageRepository. Add(chatMessage);
        //    return chatMessage;


        //}
        
        

        [HttpPost("addgroup")]
        public async Task<ChatGroup> Post([FromForm] ChatGroup group)
        {


            await _messageRepository.AddGroup(group);
            return group;


        }

        [HttpPut]
        public async Task<ChatGroup> put([FromForm] ChatGroup group)
        {


            await _messageRepository.UpdateGroup(group);
            return group;


        }


        [HttpGet("groups/{groupname}")]
        public async Task<ChatMessage> GetLastMessage( string groupname )
        {
            return await _messageRepository.GetLastMessageOfGroup(groupname);

        }


        [HttpPost()]
        public async Task<ChatMessage> Post([FromBody] ChatMessage message)
        {


            await _messageRepository.Add(message);
            return message;


        }


    }
}
