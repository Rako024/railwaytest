using Expo.Business.DTOs.ProductDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.DTOs.WishlistDtos
{
    public record WishlistItemDto
    {
        public ProductDto Product { get; set; }
       
    }
}
