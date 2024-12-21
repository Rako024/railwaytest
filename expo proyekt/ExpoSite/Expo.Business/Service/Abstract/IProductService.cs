using Expo.Business.DTOs.ProductDtos;
using Expo.Business.Service.Dtos;
using Expo.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Service.Abstract
{
    public interface IProductService
    {

        string? GetAppUserId();
        Task<Product> AddProduct(CreateProductDto productDto);
        Task<ProductDto?> GetProduct(
        Expression<Func<Product, bool>>? predicate,
        Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null
        );
        Task<List<ProductDto>> GetAllProductAsync(
            Expression<Func<Product, bool>>? predicate = null,
            Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null
        );
        Task<Product> UpdateProduct(UpdateProductDto updateProductDto);
        Task DeleteProduct(int productId);

        Task<List<ProductDto>> GetAllByPagingAsync(
           Expression<Func<Product, bool>>? predicate = null,
           Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include = null,
           Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
           int currenPage = 1,
           int pageSize = 12
       );
        Task<int> CountProduct(
            Expression<Func<Product, bool>>? predicate = null
           );
    }
}
