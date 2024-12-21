using Expo.Business.DTOs.CategoryDtos;
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
    public interface ICategoryService
    {
        Task AddCategoryAsync(CreateCategoryDTO dto);
        Task UpdateCategoryAsync(UpdateCategoryDTO dto);
        Task<List<CategoryDTO>> GetAllCategoriesAsync(
            Expression<Func<Category, bool>>? predicate = null,
            Func<IQueryable<Category>, IIncludableQueryable<Category, object>>? include = null,
            Func<IQueryable<Category>, IOrderedQueryable<Category>>? orderBy = null
        );
        Task<List<CategoryDTO>> GetAllCategoriesTreeAsync();
        Task<CategoryDTO> GetCategoryAsync(
            Expression<Func<Category, bool>>? predicate,
            Func<IQueryable<Category>, IIncludableQueryable<Category, object>>? include = null
        );
        Task DeleteCategoryAsync(int id);
    }
}
