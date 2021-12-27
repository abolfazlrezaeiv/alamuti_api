using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Polly;
using System.Security.Claims;

namespace API
{
    //[Authorize]
    public class ChatHub : Hub
    {
        private readonly MessageRepository _repository;
   

        public ChatHub(MessageRepository repository)
        {
            _repository = repository;
   
        }

      
        public async Task SendMessage(string receiverId,string senderId, string message,string? groupname1,string grouptitle,string groupImage)
        {
            var groupname2 = $"{receiverId + senderId}";
            await Groups.AddToGroupAsync(Context.ConnectionId,groupname1 ??  groupname2);

            await Clients.Group(groupname1 ?? groupname2).SendAsync("ReceiveMessage",receiverId,senderId,message, groupname1 ?? groupname2, grouptitle,groupImage);
            await _repository.AddGroup(new ChatGroup() { Name = groupname1 ?? groupname2, Title = grouptitle ,Image = groupImage});
            await _repository.AddMessageToGroup(groupname1 ?? groupname2, new ChatMessage { Sender = senderId, Reciever = receiverId, DateSended = DateTime.UtcNow, Message = message, GroupName = groupname1 ?? groupname2, });

            //await _repository.Add(new ChatMessage { Sender = senderId, Reciever = receiverId, DateSended = DateTime.UtcNow, Message = message });

        }

        public async Task ResponseMessage(string receiverId, string senderId, string message)
        {
            var groupname = $"{senderId + receiverId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupname);
            await Clients.Group(groupname).SendAsync("ReceiveMessage", receiverId, senderId, message);
            //await _repository.AddMessageToGroup(groupname, new ChatMessage { Sender = senderId, Reciever = receiverId, DateSended = DateTime.UtcNow, Message = message ,});

        }


        public async Task CreateMyGroup(string MyGroupId)
        {

           
            await Groups.AddToGroupAsync(Context.ConnectionId, MyGroupId);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task CreatenewGroup(string groupname,string title)
        {
            //var groupname = $"{receiverId + senderId}";
            await _repository.AddGroup(new ChatGroup() { Name = groupname,Title = title});

            await _repository.UpdateGroup(new ChatGroup() { Name = groupname, IsChecked = false});


            await Groups.AddToGroupAsync(Context.ConnectionId, groupname);

        }


    }
    
}