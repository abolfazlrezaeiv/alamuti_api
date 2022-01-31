using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Polly;
using System.Security.Claims;

namespace API
{
    public class ChatHub : Hub
    {
        private readonly MessageRepository _repository;

        public ChatHub(MessageRepository repository)
        {
            _repository = repository;
        }


        public async Task SendMessage(string receiverId, string senderId, string message, string groupNameFromClient, string grouptitle, string? groupImage)
        {

            //var groupnameFirstTime = $"{receiverId + senderId + grouptitle}";

            await Groups.AddToGroupAsync(Context.ConnectionId, groupNameFromClient);

            await Clients.Group(groupNameFromClient).SendAsync("ReceiveMessage", receiverId, senderId, message, groupNameFromClient, grouptitle);


            await _repository.AddMessageToGroup(groupNameFromClient, new ChatMessage { Sender = senderId, Reciever = receiverId, DateSended = DateTime.UtcNow, Message = message, GroupName = groupNameFromClient });

        }

        public async Task InitializeChat(string receiverId, string senderId, string? groupNameFromClient, string grouptitle, string? groupImage)
        {

            await Clients.Group(receiverId).SendAsync("InitializeChat", receiverId, senderId, groupNameFromClient, grouptitle);

            await _repository.AddGroup(new ChatGroup() { Name = groupNameFromClient, Title = grouptitle, Image = groupImage, IsChecked = false });

        }



        public async Task JoinToGroup(string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task CreateNewGroup(string groupname, string title)
        {
            await _repository.AddGroup(new ChatGroup() { Name = groupname, Title = title });

            await _repository.UpdateGroup(groupname);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupname);
        }
    }
}