using Expo.Business.DTOs.ProductDtos;
using Expo.Business.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpoSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOrCustomerPolicy")]
    public class BasketController : ControllerBase
    {   private readonly IBasketService _basketService;
        private readonly IProductService _productService;

        public BasketController(IBasketService basketService, IProductService productService)
        {
            _basketService = basketService;
            _productService = productService;
        }

        [HttpPost("add-basket-item")]
        public async Task<IActionResult> AddBasketItem(int productId, int count)
        {
            try
            {
                var appUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (appUserId == null)
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Message = "Profilə Giriş edin"
                    });
                ProductDto? product = await _productService.GetProduct(x=> x.Id == productId && !x.IsDeleted);
                if (product == null)
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Məhsul Tapılmadı"
                    });
                
                await _basketService.AddBasketItemAsync(appUserId, productId, count);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Product added to basket successfully"
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
        [HttpPost("confirm-basket")]
        public async Task<IActionResult> ConfirmBasket()
        {
            try
            {
                var appUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (appUserId == null)
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Message = "Profilə Giriş edin"
                    });

                await _basketService.ConfirmBasketAsync(appUserId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Basket confirmed and order created successfully"
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

        [HttpPut("decrease-basket-item-count")]
        public async Task<IActionResult> DecreaseBasketItemCount(int basketItemId)
        {
            try
            {
                await _basketService.DecreaseBasketItemCountAsync(basketItemId, 1);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Basket item count decreased successfully"
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
        [HttpDelete("delete-basket-item")]
        public async Task<IActionResult> DeleteBasketItem(int basketItemId)
        {
            try
            {
                await _basketService.DeleteBasketItemByIdAsync(basketItemId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Basket item deleted successfully"
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

        [HttpGet("get-basket-items")]
        public async Task<IActionResult> GetBasketItems()
        {
            try
            {
                var appUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (appUserId == null)
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Message = "Profilə Giriş edin"
                    });

                var basketItems = await _basketService.GetBasketItemsAsync(appUserId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = basketItems
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

    }
}
