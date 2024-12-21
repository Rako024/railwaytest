using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.DTOs.UserDtos
{
    public record ForgotPasswordDto
    {
        public string Email { get; set; }
    }
}
