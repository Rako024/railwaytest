using Expo.Core.Entities;
using Expo.Data.DAL;
using Expo.Data.Repositories.Abstracts;

namespace Expo.Data.Repositories.Concretes
{
    public class WishlistRepository : Repository<Wishlist>, IWishlistRepository
    {
        public WishlistRepository(ExpoContext context) : base(context)
        {
        }
    }
}
