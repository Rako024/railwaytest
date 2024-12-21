using Expo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.DTOs.ProductDtos
{
    public record ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public double Price { get; set; }
        public bool IsDiscount { get; set; } = false;
        public double? DiscountPrice { get; set; } = null;
        public bool IsStock { get; set; } = false;
        public int? Stock { get; set; } = null;
        public List<string>? Images { get; set; } = new List<string>();
        public DateTime CreateDate { get; set; }
        public bool IsWishlist { get; set; } = false;
    }
}
