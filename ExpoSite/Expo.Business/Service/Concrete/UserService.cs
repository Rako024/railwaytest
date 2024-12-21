using AutoMapper;
using Expo.Business.DTOs.UserDtos;
using Expo.Business.Exceptions;
using Expo.Business.Service.Abstract;
using Expo.Core.Entities;
using Expo.Core.HelperEntities;
using Expo.Data.Repositories.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Service.Concrete
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly IBasketService _basketService;
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IMailService _mailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService, ILogger<UserService> logger, IMapper mapper, IBasketService basketService, IWishlistRepository wishlistRepository, IMailService mailService, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _logger = logger;
            _mapper = mapper;
            _basketService = basketService;
            _wishlistRepository = wishlistRepository;
            _mailService = mailService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IdentityResult> RegisterAsync(UserRegisterDto userDto)
        {
            if (userDto.Password != userDto.ConfirmPassword)
                throw new GlobalAppException("Passwords do not match!");
            try
            {


                AppUser user = _mapper.Map<AppUser>(userDto);   

                var result = await _userManager.CreateAsync(user, userDto.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new GlobalAppException($"User creation failed: {errors}");
                }

                var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
                if (!roleResult.Succeeded)
                    throw new GlobalAppException("Failed to assign role.");
                Basket basket = new Basket
                {
                    AppUserId = user.Id
                };
                Basket basketResponse = await _basketService.AddBasketAsync(basket);
                
                var wishlist = new Wishlist
                {
                    AppUserId = user.Id
                };

                await _wishlistRepository.AddAsync(wishlist);
                await _wishlistRepository.Save();
                return result;
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user");
                throw new GlobalAppException("An unexpected error occurred while registering the user", ex);
            }

        }

        public async Task<TokenResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            // İstifadəçini email ilə tapın
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
            {
                throw new GlobalAppException("User not found!");
            }
            List<string>? roles =(List<string>) await _userManager.GetRolesAsync(user);
            if (roles.FirstOrDefault() == "Admin")
            {
                throw new GlobalAppException("User not found!");
            }

            // Parolu yoxlayın
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!isPasswordValid)
            {
                throw new GlobalAppException("Password or email is not correct"); // Parol səhvdirsə null qaytar
            }
            if (!user.EmailConfirmed)
            {
                throw new GlobalAppException("Email not Confirmed!");
            }
            // Token yaradın
            TokenResponseDto tokenResponse = await _tokenService.CreateToken(user);
            return tokenResponse;
        }

        public async Task<TokenResponseDto> AdminLoginAsync(LoginRequestDto loginRequest)
        {
            // İstifadəçini email ilə tapın
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
            {
                throw new GlobalAppException("User not found!");
            }
            List<string>? roles = (List<string>)await _userManager.GetRolesAsync(user);
            if (roles.FirstOrDefault() == "Customer")
            {
                throw new GlobalAppException("User not found!");
            }

            // Parolu yoxlayın
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!isPasswordValid)
            {
                throw new GlobalAppException("Password or email is not correct"); // Parol səhvdirsə null qaytar
            }
            if (!user.EmailConfirmed)
            {
                throw new GlobalAppException("Email not Confirmed!");
            }
            // Token yaradın
            TokenResponseDto tokenResponse = await _tokenService.CreateToken(user);
            return tokenResponse;
        }

        public async Task SendForgotPasswordEmailAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null || !user.EmailConfirmed)
                throw new GlobalAppException("Invalid email or email not confirmed.");

            // Generate reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Create reset link
            var resetLink = $"https://expo-proje.vercel.app/renew-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

            // Send email
            await _mailService.SendEmailAsync(new MailRequest
            {
                ToEmail = user.Email,
                Subject = "Reset Password",
                Body = $"<p>Click the link to reset your password: <a href='{resetLink}'>Reset Password</a></p>"
            });
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
                throw new GlobalAppException("Passwords do not match.");

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                throw new GlobalAppException("Invalid email.");

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            return result;
        }

        public async Task UpdateUserAsync(string fullName, string companyName, string address, string phoneNumber)
        {
            // İstifadəçini ID ilə tapın
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new GlobalAppException("User ID could not be found.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new GlobalAppException("User not found.");
            }

            bool isUpdated = false;

            // Hər bir dəyəri yoxlayın və yalnız dəyişiklik olarsa yeniləyin
            if (!string.IsNullOrEmpty(fullName) && user.FullName != fullName)
            {
                user.FullName = fullName;
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(companyName) && user.CompanyName != companyName)
            {
                user.CompanyName = companyName;
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(address) && user.Adress != address)
            {
                user.Adress = address;
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(phoneNumber) && user.PhoneNumber != phoneNumber)
            {
                user.PhoneNumber = phoneNumber;
                isUpdated = true;
            }

            if (isUpdated)
            {
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new GlobalAppException($"Failed to update user: {errors}");
                }
            }
            
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList(); 
            return _mapper.Map<IEnumerable<UserDto>>(users); 
        }


        //public override bool Equals(object? obj)
        //{
        //    return obj is UserService service &&
        //           EqualityComparer<ILogger<UserService>>.Default.Equals(_logger, service._logger);
        //}
    }
}
