using Expo.Business.Exceptions;
using Expo.Business.Service.Abstract;
using Expo.Core.Entities;
using Expo.Data.Repositories.Abstracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Service.Concrete
{
    public class ProductImageService : IProductImageService
    {
        private readonly IFileService _fileService;
        private readonly IProductImageRepository _productImageRepository;

        public ProductImageService(IFileService fileService, IProductImageRepository productImageRepository)
        {
            _fileService = fileService;
            _productImageRepository = productImageRepository;
        }
        public async Task AddProductImages(List<IFormFile> files, int productId)
        {
            if (files == null || !files.Any())
                throw new GlobalAppException("Files list is empty");

            foreach (var file in files)
            {
                // Faylı yükləyir və fayl adını alır
                var fileName = await _fileService.UploadFile(file, "pictures");

                // Yeni ProductImage obyekti yaradır
                var productImage = new ProductImage
                {
                    ProductId = productId,
                    ImageName = fileName
                };

                // Verilənlər bazasına qeyd edir
                await _productImageRepository.AddAsync(productImage);
                await _productImageRepository.Save();
            }
        }

        public async Task DeleteProductImage(ProductImage productImage)
        {
            // Faylı sil
            await _fileService.DeleteFile("pictures", productImage.ImageName);

            // Verilənlər bazasından sil
            await _productImageRepository.HardDeleteAsync(productImage);
            await _productImageRepository.Save();
        }
    }
}
