using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Expo.Business.Service.Dtos
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public double Price { get; set; }
        public bool IsDiscount { get; set; }
        public double? DiscountPrice { get; set; }
        public bool IsStock { get; set; }
        public int? Stock { get; set; }
        public List<IFormFile> Images { get; set; }
    }
}
