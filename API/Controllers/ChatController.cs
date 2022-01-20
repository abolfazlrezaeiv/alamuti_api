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
        public  IEnumerable<ChatMessageDto> Get(string groupname,[FromQuery] MessageParameters messageParameters)
        {
            var messages =  _messageRepository.GetMessages(groupname, messageParameters);

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



        //[HttpGet("groups")]
        //public async Task<IActionResult> GetGroups()
        //{
        //    var userId = User.Claims.FirstOrDefault()?.Value;
        //    return Ok(await _messageRepository.GetAllGroup(userId));
           
        //}

        [HttpGet("groupswithmessages")]
        public async Task<IActionResult> GetGroupsWithMessages()
        {
            var userId = User.Claims.FirstOrDefault()?.Value;
            var groups = await _messageRepository.GetGroupWithMessages(userId);
           
            var result = groups.Select(group=> _mapper.Map<ChatGroupDto>(group));
           
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
