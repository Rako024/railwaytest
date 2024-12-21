﻿using Expo.Business.DTOs.UserDtos;
using Expo.Business.Service.Abstract;
using Expo.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Service.Concrete
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<TokenResponseDto> CreateToken(AppUser user, int expireDate = 180)
        {
            List<Claim> claims = new List<Claim>()
             {

                 new Claim(ClaimTypes.Email, user.Email),

                 new Claim(ClaimTypes.NameIdentifier, user.Id)
                
             };
            var roles = await _userManager.GetRolesAsync(user);

            // Hər rolu ayrı-ayrılıqda `Claim` kimi əlavə etmə
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims, notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expireDate),
                signingCredentials: signingCredentials
            );
            var tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(jwtSecurityToken);
            return new TokenResponseDto ()
            {
                Token = token,
                ExpireDate = jwtSecurityToken.ValidTo,
                UserId = user.Id
            };
        }


        
    }
}
