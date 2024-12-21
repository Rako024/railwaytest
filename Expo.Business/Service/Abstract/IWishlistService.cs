using Expo.Business.DTOs;
using Expo.Business.DTOs.WishlistDtos;

namespace Expo.Business.Service.Abstract
{
    public interface IWishlistService
    {
        Task<WishlistDto> GetWishlistAsync(string appUserId);
        Task AddToWishlistAsync(string appUserId, int productId);
        Task RemoveFromWishlistAsync(string appUserId, int productId);
    }
}
