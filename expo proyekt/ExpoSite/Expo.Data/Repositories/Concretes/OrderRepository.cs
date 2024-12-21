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
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ExpoContext expoContext) : base(expoContext)
        {
        }
    }
}
