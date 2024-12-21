using Expo.Business.DTOs.CategoryDtos;
using Expo.Business.Exceptions;
using Expo.Business.Service.Abstract;
using Expo.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Expo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Add a new category
        [HttpPost("create-category")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryDTO dto)
        {
            try
            {
                await _categoryService.AddCategoryAsync(dto);
                return Ok(new {StatusCode = StatusCodes.Status201Created ,Message = "Category successfully added." });
            } catch (GlobalAppException ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }

        // Update an existing category
        [HttpPut("update-category")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDTO dto)
        {
            

            await _categoryService.UpdateCategoryAsync(dto);
            return Ok(new { StatusCode = StatusCodes.Status200OK ,Message = "Category successfully updated." });
        }

        // Get all categories with optional filters
        [HttpGet("getallcategories")]
        public async Task<IActionResult> GetAllCategories(
            [FromQuery] string? name = null,
            [FromQuery] int? superCategoryId = null)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(
                predicate: c => (string.IsNullOrEmpty(name) || c.Name.Contains(name)) &&
                                (superCategoryId == null || c.SuperCategoryId == superCategoryId));

            return Ok(new
            {
                StatusCode= StatusCodes.Status200OK,
                data = categories
            });
        }

        [HttpGet("getAllCategoriesTree")]
        public async Task<IActionResult> GetAllCategoriesTree()
        {
            var categoriesTree = await _categoryService.GetAllCategoriesTreeAsync();
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = categoriesTree
            });
        }

        // Get a specific category by ID
        [HttpGet("getCategorybyid/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryAsync(c => c.Id == id,
                 include: query => query.Include(c => c.SubCategories));
            if (category == null)
                return NotFound(new { Message = "Category not found." });

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
              data =   category
            });
        }
        [HttpGet("getCategorybyname/{name}")]
        public async Task<IActionResult> GetCategoryByName(string name)
        {
            var category = await _categoryService.GetCategoryAsync(c => c.Name == name,
                 include: query => query.Include(c => c.SubCategories));
            if (category == null)
                return NotFound(new { Message = "Category not found." });

            return Ok(category);
        }

        // Delete a category
        [HttpDelete("delete-category/{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                CategoryDTO category = await _categoryService.GetCategoryAsync(p=>p.Id == id,
                    include: query=> query.Include(c=>c.SubCategories));
                if (!category.SubCategories.IsNullOrEmpty())
                {
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = $"{category.Name} have Sub Categories. Please delete all Sub categories"
                    });
                }
            await _categoryService.DeleteCategoryAsync(id);
            return Ok(new {StatusCode = StatusCodes.Status200OK ,Message = "Category successfully deleted." });
            }catch(GlobalAppException ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }
    }
}
