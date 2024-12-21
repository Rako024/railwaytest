using Expo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Service.Abstract
{
    public interface IBasketService
    {
        Task<Basket> AddBasketAsync(Basket basket);
        Task ConfirmBasketAsync(string appUserId);
        Task AddBasketItemAsync(string appUserId, int productId, int count);

        Task DeleteBasketItemByIdAsync(int basketItemId);
        Task DecreaseBasketItemCountAsync(int basketItemId, int count);
        Task<IEnumerable<object>> GetBasketItemsAsync(string appUserId);

    }
}
