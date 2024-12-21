using Expo.Business.DTOs.UserDtos;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Service.Abstract
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(UserRegisterDto userDto);
        Task<TokenResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task SendForgotPasswordEmailAsync(ForgotPasswordDto forgotPasswordDto);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<TokenResponseDto> AdminLoginAsync(LoginRequestDto loginRequest);
        Task UpdateUserAsync(string fullName, string companyName, string address, string phoneNumber);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

    }
}
