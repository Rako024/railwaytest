using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Core.Entities
{
    public class Basket : BaseEntitiy
    {
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public bool IsConfirm { get; set; } = false; // Səbətin təsdiqlənməsi
        public ICollection<BasketItem>? BasketItems { get; set; } = new List<BasketItem>();
    }
}
