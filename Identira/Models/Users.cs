using Microsoft.AspNetCore.Identity;

namespace Identira.Models
{
    public class Users : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
