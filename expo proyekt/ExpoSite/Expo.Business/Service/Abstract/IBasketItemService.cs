using Expo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Service.Abstract
{
    public interface IBasketItemService
    {
        Task AddBasketItemAsync(string appUserId, int productId, int count);
        Task RemoveBasketItemAsync(int basketItemId);
        Task<BasketItem?> GetBasketItemAsync(Expression<Func<BasketItem, bool>> predicate);
        Task<BasketItem> UpdateBasketItemAsync(BasketItem basketItem);

    }
}
