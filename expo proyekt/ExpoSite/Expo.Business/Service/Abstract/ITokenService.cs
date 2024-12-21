using Expo.Business.DTOs.UserDtos;
using Expo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.Service.Abstract
{
    public interface ITokenService
    {
        Task<TokenResponseDto> CreateToken(AppUser user, int expireDate = 1440);

    } 
        
}
