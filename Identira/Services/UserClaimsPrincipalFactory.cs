using System.Security.Claims;
using Identira.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identira.Services
{
    public class UserClaimsPrincipalFactory : UserClaimsPrincipalFactory<Users, IdentityRole>
    {
        public UserClaimsPrincipalFactory(
            UserManager<Users> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(Users user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            if (!string.IsNullOrEmpty(user.FullName))
            {
                identity.AddClaim(new Claim("FullName", user.FullName));
            }

            return identity;
        }
    }
}
