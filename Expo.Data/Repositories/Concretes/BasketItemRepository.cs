using Expo.Core.Entities;
using Expo.Data.DAL;
using Expo.Data.Repositories.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Data.Repositories.Concretes
{
    public class BasketItemRepository : Repository<BasketItem>, IBasketItemRepository
    {
        public BasketItemRepository(ExpoContext expoContext) : base(expoContext)
        {
        }
    }
}
