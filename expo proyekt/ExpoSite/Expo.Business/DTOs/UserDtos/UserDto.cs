using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Business.DTOs.UserDtos
{
    public record UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; } 
        public string Address { get; set; } 
        public string PhoneNumber { get; set; } 
        public bool IsDeleted { get; set; } 
    }
}
