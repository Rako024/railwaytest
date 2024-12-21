using AutoMapper;
using Expo.Business.DTOs;
using Expo.Business.DTOs.WishlistDtos;
using Expo.Business.Exceptions;
using Expo.Business.Service.Abstract;
using Expo.Core.Entities;
using Expo.Data.Repositories.Abstracts;
using Expo.Data.Repositories.Concretes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Expo.Business.Service.Concrete
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        

        public WishlistService(IWishlistRepository wishlistRepository, IMapper mapper, IProductService productService, IHttpContextAccessor httpContextAccessor)
        {
            _wishlistRepository = wishlistRepository;
            _mapper = mapper;
            _productService = productService;
            
        }

        public async Task<WishlistDto> GetWishlistAsync(string appUserId)
        {
            // İstifadəçinin Wishlist-i varsa, onu gətir
            var wishlist = await _wishlistRepository.GetAsync(
                w => w.AppUserId == appUserId,
                include: q => q.Include(w => w.WishlistItems).ThenInclude(wi => wi.Product).ThenInclude(p => p.ProductImages)
            );

            // Əgər Wishlist mövcud deyilsə, avtomatik yarat
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    AppUserId = appUserId
                };

                await _wishlistRepository.AddAsync(wishlist);
                await _wishlistRepository.Save();
            }

            // Map edərək DTO qaytar
            var wishlistDto  =   _mapper.Map<WishlistDto>(wishlist);

            // Hər bir məhsulun `isWishlist` dəyərini `true` olaraq təyin edin
            foreach (var item in wishlistDto.Items)
            {
                if (item.Product != null)
                {
                    item.Product.IsWishlist = true;
                }
            }
            return wishlistDto;
        }


        public async Task AddToWishlistAsync(string appUserId, int productId)
        {
            // Məhsulu yoxla
            var product = await _productService.GetProduct(p => p.Id == productId && !p.IsDeleted);
            if (product == null)
                throw new GlobalAppException("Product not found");

            

            // İstifadəçinin mövcud Wishlist-i yoxlanılır
            var wishlist = await _wishlistRepository.GetAsync(
                w => w.AppUserId == appUserId,
                include: q => q.Include(w => w.WishlistItems)
            );

            // Əgər Wishlist mövcud deyilsə, yenisi yaradılır
            if (wishlist == null)
            {
                wishlist = new Wishlist { AppUserId = appUserId };
                await _wishlistRepository.AddAsync(wishlist);
                await _wishlistRepository.Save();
            }

            // Məhsul artıq Wishlist-dədirsə, xətanı qaytar
            if (wishlist.WishlistItems.Any(wi => wi.ProductId == productId))
                throw new GlobalAppException("Product already in wishlist");

            // Məhsulu Wishlist-ə əlavə et
            wishlist.WishlistItems.Add(new WishlistItem { ProductId = productId });
            await _wishlistRepository.Save();
        }


        public async Task RemoveFromWishlistAsync(string appUserId, int productId)
        {
            var wishlist = await _wishlistRepository.GetAsync(
                w => w.AppUserId == appUserId,
                include: q => q.Include(w => w.WishlistItems)
            );

            if (wishlist == null)
                throw new Exception("Wishlist not found");

            var item = wishlist.WishlistItems.FirstOrDefault(wi => wi.ProductId == productId);
            if (item == null)
                throw new Exception("Product not found in wishlist");

            wishlist.WishlistItems.Remove(item);
            await _wishlistRepository.Save();
        }
    }
}
