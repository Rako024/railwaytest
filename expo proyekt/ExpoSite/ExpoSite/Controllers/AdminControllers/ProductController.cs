using Expo.Business.DTOs.ProductDtos;
using Expo.Business.Exceptions;
using Expo.Business.Service.Abstract;
using Expo.Business.Service.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpoSite.Controllers.AdminControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("create-product")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateProduct(CreateProductDto dto)
        {
            try
            {
                await _productService.AddProduct(dto);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Product created!"
                });
            }catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("update-product")]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
        {
            try
            {
                var updatedProduct = await _productService.UpdateProduct(updateProductDto);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Product updated successfully",
                    
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("get-product-by-id/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                ProductDto? product = await _productService.GetProduct(x => x.Id == id && !x.IsDeleted,
                    query => query.Include(x => x.ProductImages).Include(y=>y.Category));
                if (product == null)
                {
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Product Not Found!"
                    });
                }
                return Ok(new 
                { 
                    StatusCode = StatusCodes.Status200OK,
                    data = product 
                });
            }catch(GlobalAppException ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }catch(Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("get-all-products-by-categoryId/{categoryId}")]
        public async Task<IActionResult> GetProductByCategoryId(int categoryId)
        {
            List<ProductDto?> products = await _productService.GetAllProductAsync(x => x.CategoryId == categoryId && !x.IsDeleted,
                query => query.Include(x => x.ProductImages).Include(x=>x.Category),
                x=>x.OrderBy(y=>y.Name));
            if (products == null)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Product Not Found!"
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = products.ToList()
            });
        }

        [HttpGet("get-all-products-by-name/{name}")]
        public async Task<IActionResult> GetProductByCategoryId(string name)
        {
            List<ProductDto?> products = await _productService.GetAllProductAsync(x => x.Name.ToUpper().Contains(name.ToUpper()) && !x.IsDeleted,
                query => query.Include(x => x.ProductImages).Include(x => x.Category),
                x => x.OrderBy(y => y.Name));
            if (products == null)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Product Not Found!"
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = products.ToList()
            });
        }

        [HttpGet("get-all-products")]
        public async Task<IActionResult> GetProductByCategoryId()
        {
            List<ProductDto?> products = await _productService.GetAllProductAsync(x => !x.IsDeleted,
                query => query.Include(x => x.ProductImages).Include(x => x.Category),
                x => x.OrderBy(y => y.Name));
            if (products == null)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Product Not Found!"
                });
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = products.ToList()
            });
        }

        [HttpGet("get-all-products-by-new")]
        public async Task<IActionResult> GetProductsByNew()
        {
            // Hal-hazırki tarixdən bir həftə əvvəlki tarixi hesablayırıq
            DateTime oneWeekAgo = DateTime.UtcNow.AddDays(-7);

            // Şərtə uyğun olan məhsulları əldə edirik
            List<ProductDto?> products = await _productService.GetAllProductAsync(
                x => !x.IsDeleted && x.CreatedDate >= oneWeekAgo,
                query => query.Include(x => x.ProductImages).Include(x => x.Category),
                x => x.OrderByDescending(y => y.CreatedDate) // Yenilərini əvvəl göstərmək üçün sıralama
            );

            // Əgər nəticə tapılmırsa, xəta qaytarılır
            if (products == null || !products.Any())
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "No new products found within the last week."
                });
            }

            // Tapılmış məhsullar qaytarılır
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = products.ToList()
            });
        }


        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("delete-product/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProduct(id);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Product deleted successfully"
                });
            }
            catch (GlobalAppException ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "An unexpected error occurred.",
                    Details = ex.Message
                });
            }
        }


        [HttpGet("get-all-product-fileter-and-sort-paging")]
        public async Task<IActionResult> GetAllPagingAndFilet(int? categoryId,int? lowerPrice,int? upperPrice,bool orderFlag = false,int currentPage = 1 ,int pageSize = 12)
        {
            List<ProductDto> products = new List<ProductDto>();
            int count = 0 ;
            if(categoryId == null)
            {
                if(lowerPrice == null && upperPrice == null)
                {
                    products = await _productService.GetAllByPagingAsync(x=> !x.IsDeleted, query => query.Include(x=>x.ProductImages).Include(x => x.Category), x => x.OrderBy(y => y.Price), currentPage, pageSize);
                    count = await _productService.CountProduct(x=> !x.IsDeleted);
                }
                if(lowerPrice!=null && upperPrice!=null)
                {
                    products = await _productService.GetAllByPagingAsync(x=> x.Price>=lowerPrice && x.Price<=upperPrice && !x.IsDeleted, query => query.Include(x => x.ProductImages).Include(x => x.Category), x => x.OrderBy(y => y.Price), currentPage, pageSize);
                    count = await _productService.CountProduct(x => x.Price >= lowerPrice && x.Price <= upperPrice && !x.IsDeleted);
                }
            }

            if (categoryId != null)
            {
                if (lowerPrice == null && upperPrice == null)
                {
                    products = await _productService.GetAllByPagingAsync(x=>x.CategoryId == categoryId && !x.IsDeleted, query => query.Include(x => x.ProductImages).Include(x => x.Category), x => x.OrderBy(y => y.Price), currentPage, pageSize);
                    count = await _productService.CountProduct(x => x.CategoryId == categoryId &&!x.IsDeleted);
                }
                if (lowerPrice != null && upperPrice != null)
                {
                    products = await _productService.GetAllByPagingAsync(x => x.CategoryId == categoryId && x.Price >= lowerPrice && x.Price <= upperPrice && !x.IsDeleted, query => query.Include(x => x.ProductImages).Include(x => x.Category), x => x.OrderBy(y => y.Price), currentPage, pageSize);
                    count = await _productService.CountProduct(x => x.CategoryId == categoryId && x.Price >= lowerPrice && x.Price <= upperPrice && !x.IsDeleted);
                }
            }
            if (orderFlag)
            {
               products.Reverse();
            }
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                data = new { products, count }
            });
        }
    }
}
