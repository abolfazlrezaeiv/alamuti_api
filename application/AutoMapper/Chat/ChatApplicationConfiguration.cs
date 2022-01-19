using application.DTOs.Chat;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace application.AutoMapper.Chat
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {

            CreateMap<ChatMessage, ChatMessageDto>()
                .AddTransform<byte[]>(s => s.Length < 2 ? null : s)
                .ForMember(dest =>
                    dest.DaySended,
                    opt => opt.MapFrom(src => src.DateSended.ToString()));
        }
    }
}
