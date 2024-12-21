using AutoMapper;
using Expo.Business.DTOs;
using Expo.Business.DTOs.WishlistDtos;
using Expo.Core.Entities;

namespace Expo.Business.Profiles
{
    public class WishlistMapProfiles : Profile
    {
        public WishlistMapProfiles()
        {
            // Wishlist -> WishlistDto
            CreateMap<Wishlist, WishlistDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.WishlistItems))
                .ReverseMap();

            // WishlistItem -> WishlistItemDto
            CreateMap<WishlistItem, WishlistItemDto>()
                  .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ReverseMap();
        }
    }
}
