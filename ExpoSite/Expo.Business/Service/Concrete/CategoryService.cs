using AutoMapper;
using Expo.Business.DTOs.CategoryDtos;
using Expo.Business.Exceptions;
using Expo.Business.Service.Abstract;
using Expo.Core.Entities;
using Expo.Data.Repositories.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Expo.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Add Category
        public async Task AddCategoryAsync(CreateCategoryDTO dto)
        {
            if (await _repository.Find(c => c.Name == dto.Name).AnyAsync())
                throw new GlobalAppException("Category name must be unique.");

            var category = _mapper.Map<Category>(dto);
            category.Name = category.Name.ToUpper();
            category.CreatedDate = DateTime.UtcNow;
            await _repository.AddAsync(category);
            await _repository.Save();
        }

        // Update Category
        public async Task UpdateCategoryAsync(UpdateCategoryDTO dto)
        {
            var category = await _repository.GetAsync(c => c.Id == dto.Id);
            if (category == null)
                throw new GlobalAppException("Category not found.");

            if (await _repository.Find(c => c.Name == dto.Name && c.Id != dto.Id).AnyAsync())
                throw new GlobalAppException("Category name must be unique.");

            category.Name = dto.Name.ToUpper();
            category.SuperCategoryId = dto.SuperCategoryId;
            category.LastUpdatedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(category);
            await _repository.Save();
        }

        // Get All Categories
        public async Task<List<CategoryDTO>> GetAllCategoriesAsync(
            Expression<Func<Category, bool>>? predicate = null,
            Func<IQueryable<Category>, IIncludableQueryable<Category, object>>? include = null,
            Func<IQueryable<Category>, IOrderedQueryable<Category>>? orderBy = null
        )
        {
            var categories = await _repository.GetAllAsync(
                pradicate: predicate,
                include: include,
                orderBy: orderBy
            );

            return _mapper.Map<List<CategoryDTO>>(categories);
        }




        public async Task<List<CategoryDTO>> GetAllCategoriesTreeAsync()
        {
            // Rekursiv şəkildə bütün alt kateqoriyaları yükləyirik
            var categories = await _repository.GetAllAsync(
                include: query => query.Include(c => c.SubCategories)
                                       .ThenInclude(sc => sc.SubCategories)
                                       .ThenInclude(ssc => ssc.SubCategories)
                                       .ThenInclude(ssc => ssc.SubCategories)
                                       .ThenInclude(ssc => ssc.SubCategories), // Rekursiv dərinlik üçün
                pradicate: c => c.SuperCategoryId == null // Yalnız kök kateqoriyalar
            );

            // Xəritələndiririk
            return _mapper.Map<List<CategoryDTO>>(categories);
        }

        // Get Category by ID
        public async Task<CategoryDTO> GetCategoryAsync(
            Expression<Func<Category, bool>>? predicate ,
            Func<IQueryable<Category>, IIncludableQueryable<Category, object>>? include = null
        )
        {
            var category = await _repository.GetAsync(
                pradicate: predicate,
                include: include
            );

            return _mapper.Map<CategoryDTO>(category);
        }

        // Delete Category
        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _repository.GetAsync(c => c.Id == id);
            if (category == null)
                throw new GlobalAppException("Category not found.");

            await _repository.HardDeleteAsync(category);
            await _repository.Save();
        }




       
    }
}
