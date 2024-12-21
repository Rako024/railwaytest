using Expo.Business.Exceptions;
using Expo.Business.Service.Abstract;
using Expo.Core.Entities;
using Expo.Data.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Service.Concrete
{
    public class BasketItemService : IBasketItemService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IBasketItemRepository _repository;
        public BasketItemService(IBasketRepository basketRepository, IBasketItemRepository repository)
        {
            _basketRepository = basketRepository;
            _repository = repository;
        }

        public async Task AddBasketItemAsync(string appUserId, int productId, int count)
        {
            // Mövcud səbəti tap
            var basket = await _basketRepository.GetAsync(
                b => b.AppUserId == appUserId && !b.IsConfirm,
                include: query => query.Include(b => b.BasketItems)
            );

            // Əgər səbət mövcud deyilsə, yeni səbət yaradılır
            if (basket == null)
            {
                basket = new Basket
                {
                    AppUserId = appUserId,
                    IsConfirm = false,
                    BasketItems = new List<BasketItem>()
                };

                await _basketRepository.AddAsync(basket);
                await _basketRepository.Save();
            }

            // Səbətdə məhsul varsa, miqdarı artırılır, yoxdursa, yeni məhsul əlavə olunur
            var existingItem = basket.BasketItems.FirstOrDefault(bi => bi.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Count += count; // Miqdarı artır
            }
            else
            {
                basket.BasketItems.Add(new BasketItem
                {
                    ProductId = productId,
                    Count = count
                });
            }

            // Səbəti saxla
            await _basketRepository.Save();
        }


        public async Task RemoveBasketItemAsync(int basketItemId)
        {
            BasketItem item = await _repository.GetAsync(x=>x.Id == basketItemId);
            if(item == null)
            {
                throw new GlobalAppException("Basket Item Not Found!");
            }
            await _repository.HardDeleteByIdAsync(basketItemId);
            await _repository.Save();
        }

        public async Task<BasketItem?> GetBasketItemAsync(Expression<Func<BasketItem, bool>> predicate)
        {
            return await _repository.GetAsync(predicate);
        }

        public async Task<BasketItem> UpdateBasketItemAsync(BasketItem basketItem)
        {
            var updatedItem = await _repository.UpdateAsync(basketItem);
            await _repository.Save();
            return updatedItem;
        }

    }
}
