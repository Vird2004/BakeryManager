using Microsoft.AspNetCore.Identity;
using System.Data.Common;

namespace BakeryManager.Models
{
    public class AppUserModel : IdentityUser
    {
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? Age { get; set; }

        public string PhoneNumber { get; set; }

        public string RoleId { get; set; }
    }
    
    }

