using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.DTOs.ProductDtos
{
    public record UpdateProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public double Price { get; set; }
        public bool IsDiscount { get; set; }
        public double? DiscountPrice { get; set; }
        public bool IsStock { get; set; }
        public int? Stock { get; set; }
        public List<IFormFile>? Images { get; set; }
        public List<string>? DeleteImageName { get; set; }
    }
}
