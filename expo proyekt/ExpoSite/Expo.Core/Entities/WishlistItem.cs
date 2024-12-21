using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Core.Entities
{
    public class WishlistItem : BaseEntitiy
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int WishlistId { get; set; }
        public Wishlist? Wishlist { get; set; }
    }
}
