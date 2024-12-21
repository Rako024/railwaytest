using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Core.Entities
{
    public class Product : BaseEntitiy
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public double Price { get; set; }
        public bool IsDiscount { get; set; } = false;
        public double? DiscountPrice { get; set; } = null;
        public bool IsStock { get; set; } = false;
        public int? Stock { get; set; } = null;
        public List<ProductImage>? ProductImages { get; set; } = new List<ProductImage>();

    }
}
