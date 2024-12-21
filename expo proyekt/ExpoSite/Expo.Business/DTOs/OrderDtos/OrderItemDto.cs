using Expo.Business.DTOs.ProductDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.DTOs.OrderDtos
{
    public record OrderItemDto
    {
        public int ProductId { get; set; }
        public ProductDto Product { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
