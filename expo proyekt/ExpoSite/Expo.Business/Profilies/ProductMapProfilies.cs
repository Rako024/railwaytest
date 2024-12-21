using AutoMapper;
using Expo.Business.DTOs.CategoryDtos;
using Expo.Business.DTOs.ProductDtos;
using Expo.Business.Service.Dtos;
using Expo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Profilies
{
    public class ProductMapProfilies : Profile
    {
        public ProductMapProfilies() 
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.IsDiscount, opt => opt.MapFrom(src => src.IsDiscount))
                .ForMember(dest => dest.DiscountPrice, opt => opt.MapFrom(src => src.DiscountPrice))
                .ForMember(dest => dest.IsStock, opt => opt.MapFrom(src => src.IsStock))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                            src.ProductImages != null
                                ? src.ProductImages.Select(pi => pi.ImageName).ToList()
                                : new List<string>()))
                .ForMember(dest => dest.CreateDate, opt=> opt.MapFrom(src=> src.CreatedDate))
                .ReverseMap();

            CreateMap<Product, CreateProductDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))

                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.IsDiscount, opt => opt.MapFrom(src => src.IsDiscount))
                .ForMember(dest => dest.DiscountPrice, opt => opt.MapFrom(src => src.DiscountPrice))
                .ForMember(dest => dest.IsStock, opt => opt.MapFrom(src => src.IsStock))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
                .ReverseMap();

            CreateMap<Product, UpdateProductDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.IsDiscount, opt => opt.MapFrom(src => src.IsDiscount))
                .ForMember(dest => dest.DiscountPrice, opt => opt.MapFrom(src => src.DiscountPrice))
                .ForMember(dest => dest.IsStock, opt => opt.MapFrom(src => src.IsStock))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
                .ReverseMap();



        }
    }
}
