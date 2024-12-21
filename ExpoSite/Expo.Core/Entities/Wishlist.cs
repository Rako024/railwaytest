using System.Collections.Generic;

namespace Expo.Core.Entities
{
    public class Wishlist : BaseEntitiy
    {
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }

    
}
