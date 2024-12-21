using AutoMapper;
using Expo.Business.DTOs;
using Expo.Business.DTOs.OrderDtos;
using Expo.Core.Entities;

namespace Expo.Business.Profiles
{
    public class OrderMapProfiles : Profile
    {
        public OrderMapProfiles()
        {
            // Order -> OrderDto
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AppUserId, opt => opt.MapFrom(src => src.AppUserId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.AppUserFullName, opt => opt.MapFrom(src => src.AppUser.FullName))
                .ForMember(dest => dest.AppUserEmail, opt => opt.MapFrom(src => src.AppUser.Email))
                .ForMember(dest => dest.AppUserPhoneNumber, opt => opt.MapFrom(src => src.AppUser.PhoneNumber))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => (double)src.TotalPrice)) // Decimal-dan Double-a çevrilir
                 .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                .ReverseMap(); // Reverse mapping dəstəklənir

            // OrderItem -> OrderItemDto
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                 .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => (double)src.Price)) // Decimal-dan Double-a çevrilir
                .ReverseMap(); // Reverse mapping dəstəklənir
        }
    }
}
