using AutoMapper;
using Expo.Business.DTOs.CategoryDtos;
using Expo.Business.DTOs.UserDtos;
using Expo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Profilies
{
    public class CategoryMapProfilies : Profile
    {
        public CategoryMapProfilies()
        {
            
            CreateMap<Category, CategoryDTO>()
                .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories))
                .ForMember(dest => dest.Name, opt=> opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.SuperCategoryId, opt=> opt.MapFrom(src=>src.SuperCategoryId))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();

            CreateMap<Category, CreateCategoryDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.SuperCategoryId, opt => opt.MapFrom(src => src.SuperCategoryId))
                .ReverseMap();

            CreateMap<Category, UpdateCategoryDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.SuperCategoryId, opt => opt.MapFrom(src => src.SuperCategoryId))
                .ReverseMap();

        }
    }
}
