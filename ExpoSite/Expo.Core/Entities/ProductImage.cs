using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Core.Entities
{
    public class ProductImage:BaseEntitiy
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public string ImageName { get; set; }

    }
}
