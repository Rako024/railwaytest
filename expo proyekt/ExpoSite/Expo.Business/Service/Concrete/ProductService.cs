using AutoMapper;
using Expo.Business.DTOs.ProductDtos;
using Expo.Business.Exceptions;
using Expo.Business.Service.Abstract;
using Expo.Business.Service.Dtos;
using Expo.Core.Entities;
using Expo.Data.Repositories.Abstracts;
using Expo.Data.Repositories.Concretes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Service.Concrete
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageService _productImageService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWishlistRepository _wishlistRepository;

        public ProductService(IProductRepository productRepository, IProductImageService productImageService, IMapper mapper, IHttpContextAccessor httpContextAccessor, IWishlistRepository wishlistRepository)
        {
            _productRepository = productRepository;
            _productImageService = productImageService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _wishlistRepository = wishlistRepository;
        }

        


        public async Task<Product> AddProduct(CreateProductDto productDto)
        {
            // Yeni məhsul yaradılır.
            var product = _mapper.Map<Product>(productDto);
            
            product.CreatedDate = DateTime.UtcNow;
            // Məhsulu bazaya əlavə edir.
            await _productRepository.AddAsync(product);
            await _productRepository.Save();

            // Şəkilləri yükləyir və əlavə edir.
            if (productDto.Images != null && productDto.Images.Any())
            {
                await _productImageService.AddProductImages(
                    productDto.Images,
                   
                    product.Id
                );

                
            }

            return product;
        }


        public async Task<ProductDto?> GetProduct(
        Expression<Func<Product, bool>>? predicate,
        Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null
        )
        {
            if (predicate == null)
                throw new GlobalAppException("Predicate cannot be null");
            Product pro = await _productRepository.GetAsync(predicate, include);
            if(pro == null)
            {
                throw new GlobalAppException("Product not found!");
            }
            ProductDto product = _mapper.Map<ProductDto>(pro);
            // AppUserId-ni avtomatik alırıq
            var appUserId = GetAppUserId();

            // İstifadəçi daxil olubsa və Wishlist-də məhsul varsa, IsWishlist true edirik
            if (!string.IsNullOrEmpty(appUserId))
            {
                var wishlist = await _wishlistRepository.GetAsync(
                    w => w.AppUserId == appUserId,
                    include: q => q.Include(w => w.WishlistItems)
                );

                if (wishlist != null && wishlist.WishlistItems.Any(wi => wi.ProductId == product.Id))
                {
                    product.IsWishlist = true;
                }
            }
            return product;
        }

        public async Task<List<ProductDto>> GetAllProductAsync(
            Expression<Func<Product, bool>>? predicate = null,
            Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null
        )
        {
            var products = await _productRepository.GetAllAsync(predicate, include, orderBy);

            // Dto-ya çevir
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            // AppUserId-ni avtomatik alırıq
            var appUserId = GetAppUserId();

            // İstifadəçi daxil olubsa, hər məhsul üçün IsWishlist dəyərini yoxlayırıq
            if (!string.IsNullOrEmpty(appUserId))
            {
                var wishlist = await _wishlistRepository.GetAsync(
                    w => w.AppUserId == appUserId,
                    include: q => q.Include(w => w.WishlistItems)
                );

                if (wishlist != null)
                {
                    foreach (var productDto in productDtos)
                    {
                        if (wishlist.WishlistItems.Any(wi => wi.ProductId == productDto.Id))
                        {
                            productDto.IsWishlist = true;
                        }
                    }
                }
            }

            return productDtos;
        }
        public async Task<List<ProductDto>> GetAllByPagingAsync(
            Expression<Func<Product, bool>>? predicate = null,
            Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
            int currenPage = 1,
            int pageSize = 12

        )
        {
            var products = await _productRepository.GetAllByPagingAsync(predicate, include, orderBy,false,currenPage,pageSize);

            // Dto-ya çevir
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            return productDtos;
        }


        public async Task<Product> UpdateProduct(UpdateProductDto updateProductDto)
        {
            // Mövcud məhsulu tap
            var existingProduct = await _productRepository.GetAsync(p => p.Id == updateProductDto.Id,
                include: query => query.Include(p => p.ProductImages));

            if (existingProduct == null)
                throw new GlobalAppException("Product not found");

            // Məhsulu yenilə
            existingProduct.Name = updateProductDto.Name;
            existingProduct.Description = updateProductDto.Description;
            existingProduct.CategoryId = updateProductDto.CategoryId;
            existingProduct.Price = updateProductDto.Price;
            existingProduct.IsDiscount = updateProductDto.IsDiscount;
            existingProduct.DiscountPrice = updateProductDto.DiscountPrice;
            existingProduct.IsStock = updateProductDto.IsStock;
            existingProduct.Stock = updateProductDto.Stock;
            existingProduct.LastUpdatedDate = DateTime.UtcNow;

            // Şəkilləri sil
            if (updateProductDto.DeleteImageName != null && updateProductDto.DeleteImageName.Any())
            {
                foreach (var imageName in updateProductDto.DeleteImageName)
                {
                    var productImage = existingProduct.ProductImages.FirstOrDefault(pi => pi.ImageName == imageName);
                    if (productImage != null)
                    {
                        await _productImageService.DeleteProductImage(productImage);
                    }
                }
            }

            // Yeni şəkilləri əlavə et
            if (updateProductDto.Images != null && updateProductDto.Images.Any())
            {
                await _productImageService.AddProductImages(updateProductDto.Images, existingProduct.Id);
            }

            // Yenilənmiş məhsulu saxla
            await _productRepository.UpdateAsync(existingProduct);
            await _productRepository.Save();

            return existingProduct;
        }


        public async Task DeleteProduct(int productId)
        {
            // Mövcud məhsulu tap
            var existingProduct = await _productRepository.GetAsync(
                p => p.Id == productId,
                include: query => query.Include(p => p.ProductImages) // Lazım olarsa əlaqəli obyektləri də gətirə bilər
            );

            if (existingProduct == null)
                throw new GlobalAppException("Product not found");

            // Məhsulun IsDeleted sahəsini true edirik
            existingProduct.IsDeleted = true;
            existingProduct.DeletedDate = DateTime.UtcNow;
            // Məhsulu saxlayırıq
            await _productRepository.UpdateAsync(existingProduct);
            await _productRepository.Save();
        }
        public async Task<int> CountProduct(
            Expression<Func<Product, bool>>? predicate = null
           )
        {
            int count = await _productRepository.CountAsync(predicate);
            return count;
        }

        public string? GetAppUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
