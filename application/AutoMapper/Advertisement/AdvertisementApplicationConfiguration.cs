using application.DTOs;
using application.DTOs.Advertisement;
using AutoMapper;
using Domain.Entities;

namespace application.AutoMapper
{
    public class AdvertisementProfile : Profile
    {
        public AdvertisementProfile()
        { 
            CreateMap<Advertisement, AdvertisementDto>()
                .AddTransform<byte[]>(s => s.Length < 2 ? null : s)
                .ForMember(dest =>
                    dest.DaySended,
                    opt => opt.MapFrom(src => src.DatePosted.ToString()));

            CreateMap<Advertisement, AdvertisementDetailDto>()
                .AddTransform<byte[]>(s => s.Length < 2 ? null : s)
                .ForMember(dest =>
                    dest.DaySended,
                    opt => opt.MapFrom(src => src.DatePosted.ToString()))
                .AfterMap<MapPhoneNumber>();

            CreateMap<Advertisement, UserAdvertisementDto>()
              .AddTransform<byte[]>(s => s.Length < 2 ? null : s)
              .ForMember(dest =>
                  dest.DaySended,
                  opt => opt.MapFrom(src => src.DatePosted.ToString()));
        }
    }
}
