using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expo.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; } = null;
        public string? CompanyName { get; set; } = null;
        public string? Adress { get; set; } = null;
        public string? PhoneNumber { get; set; } = null;    
        public bool IsDeleted { get; set; } = false;
        
    }
}
