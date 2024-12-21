using Expo.Business.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpoSite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOrCustomerPolicy")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet("get-wishlist")]
        public async Task<IActionResult> GetWishlist()
        {
            var appUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (appUserId == null)
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Profilə Giriş edin"
                });
            try
            {
                var wishlist = await _wishlistService.GetWishlistAsync(appUserId);
                return Ok(new { StatusCode = StatusCodes.Status200OK, data = wishlist });
            }catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var appUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (appUserId == null)
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Profilə Giriş edin"
                }); 
            try
            {
            await _wishlistService.AddToWishlistAsync(appUserId, productId);
            return Ok(new {StatusCode = StatusCodes.Status201Created ,Message = "Product added to wishlist" });

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

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var appUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (appUserId == null)
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Profilə Giriş edin"
                });
            try
            {

            await _wishlistService.RemoveFromWishlistAsync(appUserId, productId);
            return Ok(new {StatusCode = StatusCodes.Status200OK ,Message = "Product removed from wishlist" });
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
