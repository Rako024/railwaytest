using Expo.Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Service.Abstract
{
    public interface IProductImageService
    {
        Task AddProductImages(List<IFormFile> files, int productId);
        Task DeleteProductImage(ProductImage productImage);
    }
}
