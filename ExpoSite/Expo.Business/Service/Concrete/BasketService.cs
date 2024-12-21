using Expo.Business.DTOs.ProductDtos;
using Expo.Business.Exceptions;
using Expo.Business.Service.Abstract;
using Expo.Core.Entities;
using Expo.Data.Repositories.Abstracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Service.Concrete
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IProductService _productService;
        private readonly IBasketItemService _basketItemService;
        private readonly IOrderRepository _orderRepository;

        public BasketService(IBasketRepository basketService, UserManager<AppUser> userManager, IProductService productService, IBasketItemService basketItemService, IOrderRepository orderRepository)
        {
            _basketRepository = basketService;
            _userManager = userManager;
            _productService = productService;
            _basketItemService = basketItemService;
            _orderRepository = orderRepository;
        }

        public async Task<Basket> AddBasketAsync(Basket basket)
        {
            if (basket == null)
                throw new GlobalAppException("Basket Is Null");
            await _basketRepository.AddAsync(basket);
            await _basketRepository.Save();
            return basket;
        }

        public async Task AddBasketItemAsync(string appUserId, int productId, int count)
        {
            if (appUserId == null)
                throw new GlobalAppException("AppUser id is null!");
            AppUser? user = await _userManager.FindByIdAsync(appUserId);
            if(user == null)
                throw new GlobalAppException("User is Not Found!");
            ProductDto? product = await _productService.GetProduct(x=>x.Id == productId && !x.IsDeleted);
            if (product == null)
                throw new GlobalAppException("Product is Not Found!");
            await _basketItemService.AddBasketItemAsync(appUserId,productId,count);
        }
        public async Task ConfirmBasketAsync(string appUserId)
        {
            var basket = await _basketRepository.GetAsync(
                   b => b.AppUserId == appUserId && !b.IsConfirm,
                   include: q => q.Include(b => b.BasketItems).ThenInclude(bi => bi.Product)
               );

            if (basket == null || basket.BasketItems == null || !basket.BasketItems.Any())
            {
                var newBasket1 = new Basket
                {
                    AppUserId = appUserId,
                    IsConfirm = false
                };

                await _basketRepository.AddAsync(newBasket1);
                await _basketRepository.Save();
                throw new GlobalAppException("Səbət Tapılmadı!");
            }
               

            // Yeni sifariş yaradılır
            var order = new Order
            {
                AppUserId = basket.AppUserId,
                CreatedDate = DateTime.UtcNow,
                TotalPrice = basket.BasketItems.Sum(bi =>
                    bi.Product.IsDiscount && bi.Product.DiscountPrice.HasValue
                        ? bi.Product.DiscountPrice.Value * bi.Count // Endirim qiymətindən hesabla
                        : bi.Product.Price * bi.Count // Normal qiymətdən hesabla
    ),
                OrderItems = basket.BasketItems.Select(bi => new OrderItem
                {
                    ProductId = bi.ProductId,
                    CreatedDate = DateTime.UtcNow,
                    Count = bi.Count,
                    Price = bi.Product.IsDiscount && bi.Product.DiscountPrice.HasValue
                        ? bi.Product.DiscountPrice.Value // Endirim qiyməti
                        : bi.Product.Price // Normal qiymət
                }).ToList()
            };

            // Səbəti təsdiqləyirik
            basket.IsConfirm = true;

            // Sifarişi saxlayırıq
            await _orderRepository.AddAsync(order);
            await _orderRepository.Save();

            // Təsdiqlənmiş səbəti saxlayırıq
            await _basketRepository.UpdateAsync(basket);
            await _basketRepository.Save();

            // Yeni boş səbət yarat
            var newBasket = new Basket
            {
                AppUserId = appUserId,
                IsConfirm = false
            };

            await _basketRepository.AddAsync(newBasket);
            await _basketRepository.Save();
        }

        public async Task DeleteBasketItemByIdAsync(int basketItemId)
        {
            if (basketItemId <= 0)
                throw new GlobalAppException("BasketItem id is invalid!");

            await _basketItemService.RemoveBasketItemAsync(basketItemId);
        }

        public async Task DecreaseBasketItemCountAsync(int basketItemId, int count)
        {
            if (basketItemId <= 0)
                throw new GlobalAppException("BasketItem id is invalid!");
            if (count <= 0)
                throw new GlobalAppException("Count must be greater than zero!");

            var basketItem = await _basketItemService.GetBasketItemAsync(x => x.Id == basketItemId);
            if (basketItem == null)
                throw new GlobalAppException("BasketItem not found!");

            if (basketItem.Count <= count)
            {
                // Əgər `count` azalanda sıfır və ya mənfi olursa, elementi tam sil
                throw new GlobalAppException("Basketdə Məhsul sayı 0 və ya mənfi ədəd ola bilməz!");
            }
            else
            {
                basketItem.Count -= count;
                await _basketItemService.UpdateBasketItemAsync(basketItem);
            }
            await _basketRepository.Save();
        }
        public async Task<IEnumerable<object>> GetBasketItemsAsync(string appUserId)
        {
            if (string.IsNullOrEmpty(appUserId))
                throw new GlobalAppException("AppUser id is null!");

            var basket = await _basketRepository.GetAsync(
                x => x.AppUserId == appUserId && !x.IsConfirm,
                include: query => query
                    .Include(b => b.BasketItems)
                    .ThenInclude(bi => bi.Product)
                    .ThenInclude(p => p.ProductImages) // Şəkillər əlavə olunur
            );

            if (basket == null || basket.BasketItems == null || !basket.BasketItems.Any())
            {
                List<object> list = new System.Collections.Generic.List<object>();  
                return list;
            }

            return basket.BasketItems.Select(bi => new
            {
                BasketItemId = bi.Id,
                ProductId = bi.ProductId,
                ProductName = bi.Product?.Name,
                Count = bi.Count,
                Price = bi.Product?.DiscountPrice ?? bi.Product?.Price,
                TotalPrice = bi.Count * (bi.Product?.DiscountPrice ?? bi.Product?.Price),
                Images = bi.Product?.ProductImages?.Select(img => img.ImageName).ToList(), // Şəkillərin adları
                TotalDiscount = bi.Product?.DiscountPrice != null && bi.Product.DiscountPrice < bi.Product.Price
                    ? bi.Count * (bi.Product.Price - bi.Product.DiscountPrice.Value) // Endirim miqdarı
                    : 0, // Əgər DiscountPrice yoxdursa və ya Price-dan böyükdürsə, endirim 0 olur
            });
        }


    }
}
