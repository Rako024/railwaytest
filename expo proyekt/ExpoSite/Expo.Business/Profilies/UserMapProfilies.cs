using AutoMapper;
using Expo.Business.DTOs.UserDtos;
using Expo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Profilies
{
    public class UserMapProfilies : Profile
    {
        public UserMapProfilies() 
        {
            CreateMap<UserRegisterDto, AppUser>()
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(d => d.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(d => d.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(d => d.Adress, opt => opt.MapFrom(src => src.Address))
                .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber)).ReverseMap();

            CreateMap<UpdateUserRequestDto, AppUser>()
               .ForMember(d => d.FullName, opt => opt.MapFrom(src => src.FullName))
               .ForMember(d => d.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
               .ForMember(d => d.Adress, opt => opt.MapFrom(src => src.Address))
               .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber)).ReverseMap();

            CreateMap<UserDto, AppUser>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(d => d.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(d => d.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(d => d.Adress, opt => opt.MapFrom(src => src.Address))
                .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(d => d.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted)).ReverseMap();    
        }
    }
}
